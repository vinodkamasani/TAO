using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;
using TAO.Application.AssessmentQuestions.FollowUp;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.CandidateResponse;

internal sealed class RecordCandidateResponseCommandHandler
    : IRequestHandler<
        RecordCandidateResponseCommand,
        Result<GenerateFollowUpResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    private readonly ICurrentUser _currentUser;

    public RecordCandidateResponseCommandHandler(
        IApplicationDbContext context,
        ISender sender,
        ICurrentUser currentUser)
    {
        _context = context;
        _sender = sender;
        _currentUser = currentUser;
    }

    public async Task<Result<GenerateFollowUpResponse>> Handle(
        RecordCandidateResponseCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null ||
            _currentUser.OrganizationId is null)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentEvaluation.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId = _currentUser.OrganizationId.Value;

        var question = await (
      from q in _context.Set<AssessmentQuestion>()

      join sessionRoundLocal in _context.Set<AssessmentSessionRound>()
          on q.AssessmentSessionRoundId equals sessionRoundLocal.Id

      join session in _context.Set<AssessmentSession>()
          on sessionRoundLocal.AssessmentSessionId equals session.Id

      join candidateApplication in _context.Set<CandidateApplication>()
          on session.CandidateApplicationId equals candidateApplication.Id

      where q.Id == request.AssessmentQuestionId
            && candidateApplication.OrganizationId == organizationId

      select q
  ).FirstOrDefaultAsync(cancellationToken);
       

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
                    "A candidate response can only be recorded for an in-progress question."));
        }

        if (question.Conversation is null)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.ConversationNotInitialized",
                    "The assessment question does not have an initialized conversation."));
        }

        JsonArray conversation;

        try
        {
            conversation = JsonNode.Parse(
                    question.Conversation.Value)
                as JsonArray
                ?? throw new JsonException(
                    "Conversation must be a JSON array.");
        }
        catch (JsonException)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.InvalidConversation",
                    "The stored assessment question conversation is invalid."));
        }

        conversation.Add(
            new JsonObject
            {
                ["role"] = "candidate",
                ["content"] = request.Response
            });

        var updatedConversation = conversation.ToJsonString(
            new JsonSerializerOptions
            {
                WriteIndented = false
            });

        question.UpdateConversation(
            ConversationContent.Create(
                updatedConversation));

        await _context.SaveChangesAsync(
            cancellationToken);

        var followUpResult = await _sender.Send(
       new GenerateFollowUpCommand(question.Id),
       cancellationToken);

        if (followUpResult.IsFailure &&
            followUpResult.Error?.Code ==
                "AssessmentQuestion.FollowUpLimitReached")
        {
            return Result<GenerateFollowUpResponse>.Success(
                new GenerateFollowUpResponse(null));
        }

        return followUpResult;
    }
}