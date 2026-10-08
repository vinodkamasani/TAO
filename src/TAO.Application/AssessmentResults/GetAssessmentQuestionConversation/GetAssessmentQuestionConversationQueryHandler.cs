using System.Text.Json;
using System.Text.Json.Nodes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentQuestionConversation;

internal sealed class GetAssessmentQuestionConversationQueryHandler
    : IRequestHandler<
        GetAssessmentQuestionConversationQuery,
        Result<GetAssessmentQuestionConversationResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetAssessmentQuestionConversationQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<GetAssessmentQuestionConversationResponse>> Handle(
        GetAssessmentQuestionConversationQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result<GetAssessmentQuestionConversationResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResults.Unauthorized",
                    "Authentication is required."));
        }

        if (!_currentUser.UserId.HasValue)
        {
            return Result<GetAssessmentQuestionConversationResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResults.UserNotFound",
                    "The authenticated user could not be resolved."));
        }

        if (!_currentUser.OrganizationId.HasValue)
        {
            return Result<GetAssessmentQuestionConversationResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResults.OrganizationNotFound",
                    "The authenticated user's organization could not be resolved."));
        }

        if (_currentUser.Role is not UserRole.Recruiter
            and not UserRole.HiringManager)
        {
            return Result<GetAssessmentQuestionConversationResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResults.Forbidden",
                    "Only recruiters and hiring managers can view assessment results."));
        }

        var organizationId = _currentUser.OrganizationId.Value;

        var questionData = await (
            from assessmentQuestion in _context
                .Set<AssessmentQuestion>()

            join sessionRound in _context
                .Set<AssessmentSessionRound>()
                on assessmentQuestion.AssessmentSessionRoundId
                    equals sessionRound.Id

            join assessmentSession in _context
                .Set<AssessmentSession>()
                on sessionRound.AssessmentSessionId
                    equals assessmentSession.Id

            join candidateApplication in _context
                .Set<CandidateApplication>()
                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentQuestion.Id == request.QuestionId
                  && assessmentSession.Id == request.AssessmentSessionId
                  && candidateApplication.OrganizationId == organizationId

            select new
            {
                Question = assessmentQuestion,
                RoundId = sessionRound.Id
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (questionData is null)
        {
            return Result<GetAssessmentQuestionConversationResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.QuestionId}' was not found."));
        }

        var conversation = questionData.Question.Conversation;

        if (conversation is null)
        {
            return Result<GetAssessmentQuestionConversationResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.ConversationNotFound",
                    "No conversation is available for this assessment question."));
        }

        var messagesResult = ParseConversation(
            conversation.Value);

        if (messagesResult.IsFailure)
        {
            return Result<GetAssessmentQuestionConversationResponse>.Failure(
                messagesResult.Error!);
        }

        return Result<GetAssessmentQuestionConversationResponse>.Success(
            new GetAssessmentQuestionConversationResponse(
                request.AssessmentSessionId,
                questionData.RoundId,
                request.QuestionId,
                messagesResult.Value!));
    }

    private static Result<IReadOnlyCollection<AssessmentConversationMessage>>
        ParseConversation(string json)
    {
        try
        {
            var nodes = JsonNode.Parse(json)?.AsArray();

            if (nodes is null)
            {
                return Result<IReadOnlyCollection<AssessmentConversationMessage>>.Failure(
                    Error.Validation(
                        "AssessmentQuestion.InvalidConversation",
                        "The stored assessment conversation is invalid."));
            }

            var messages = new List<AssessmentConversationMessage>();

            foreach (var node in nodes)
            {
                if (node is not JsonObject message)
                {
                    continue;
                }

                var role = message["role"]?.GetValue<string>();
                var content = message["content"]?.GetValue<string>();

                if (string.IsNullOrWhiteSpace(role)
                    || string.IsNullOrWhiteSpace(content))
                {
                    continue;
                }

                messages.Add(
                    new AssessmentConversationMessage(
                        role,
                        content));
            }

            return Result<IReadOnlyCollection<AssessmentConversationMessage>>
                .Success(messages);
        }
        catch (JsonException)
        {
            return Result<IReadOnlyCollection<AssessmentConversationMessage>>.Failure(
                Error.Validation(
                    "AssessmentQuestion.InvalidConversation",
                    "The stored assessment conversation contains invalid JSON."));
        }
    }
}