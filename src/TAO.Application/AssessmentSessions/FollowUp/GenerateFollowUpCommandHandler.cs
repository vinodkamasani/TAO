using System.Text.Json;
using System.Text.Json.Nodes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.FollowUp;

internal sealed class GenerateFollowUpCommandHandler
    : IRequestHandler<
        GenerateFollowUpCommand,
        Result<GenerateFollowUpResponse>>
{
    private const int MaxFollowUps = 2;

    private readonly IApplicationDbContext _context;
    private readonly IAssessmentFollowUpGenerator _followUpGenerator;

    public GenerateFollowUpCommandHandler(
        IApplicationDbContext context,
        IAssessmentFollowUpGenerator followUpGenerator)
    {
        _context = context;
        _followUpGenerator = followUpGenerator;
    }

    public async Task<Result<GenerateFollowUpResponse>> Handle(
        GenerateFollowUpCommand request,
        CancellationToken cancellationToken)
    {
        var question = await _context
            .Set<AssessmentQuestion>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentQuestionId,
                cancellationToken);

        if (question is null)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.AssessmentQuestionId}' was not found."));
        }

        if (question.Status != AssessmentQuestionStatus.InProgress)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.NotInProgress",
                    "A follow-up can only be generated for an in-progress assessment question."));
        }

        if (question.Conversation is null)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.ConversationNotInitialized",
                    "The assessment question conversation has not been initialized."));
        }

        var conversationResult = ParseConversation(
            question.Conversation.Value);

        if (conversationResult.IsFailure)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                conversationResult.Error!);
        }

        var conversation = conversationResult.Value!;

        var followUpCount = CountFollowUps(conversation);

        if (followUpCount >= MaxFollowUps)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.FollowUpLimitReached",
                    "The maximum number of follow-up questions has been reached."));
        }

        var sessionRound = await _context
            .Set<AssessmentSessionRound>()
            .FirstOrDefaultAsync(
                x => x.Id == question.AssessmentSessionRoundId,
                cancellationToken);

        if (sessionRound is null)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    $"Assessment session round '{question.AssessmentSessionRoundId}' was not found."));
        }

        var generationResult = await _followUpGenerator.GenerateAsync(
            question,
            sessionRound,
            cancellationToken);

        if (generationResult.IsFailure)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                generationResult.Error!);
        }

        var followUpQuestion =
            generationResult.Value!.Response.Question;

        conversation.Add(
            new JsonObject
            {
                ["role"] = "assistant",
                ["content"] = followUpQuestion
            });

        var updatedConversation =
            conversation.ToJsonString();

        question.UpdateConversation(
            ConversationContent.Create(
                updatedConversation));

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result<GenerateFollowUpResponse>.Success(
            new GenerateFollowUpResponse(
                followUpQuestion));
    }

    private static Result<JsonArray> ParseConversation(
        string conversationJson)
    {
        try
        {
            var conversation =
                JsonNode.Parse(conversationJson) as JsonArray;

            if (conversation is null)
            {
                return Result<JsonArray>.Failure(
                    Error.Validation(
                        "AssessmentQuestion.InvalidConversation",
                        "The assessment question conversation must be a JSON array."));
            }

            return Result<JsonArray>.Success(
                conversation);
        }
        catch (JsonException)
        {
            return Result<JsonArray>.Failure(
                Error.Validation(
                    "AssessmentQuestion.InvalidConversation",
                    "The assessment question conversation contains invalid JSON."));
        }
    }

    private static int CountFollowUps(
        JsonArray conversation)
    {
        return conversation
            .Skip(1)
            .Count(message =>
                message?["role"]?.GetValue<string>()
                    .Equals(
                        "assistant",
                        StringComparison.OrdinalIgnoreCase) == true);
    }
}