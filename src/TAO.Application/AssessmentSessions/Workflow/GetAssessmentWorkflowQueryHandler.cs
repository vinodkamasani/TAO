using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentSessions.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Workflow;

public sealed class GetAssessmentWorkflowQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetAssessmentWorkflowQuery,
        Result<AssessmentWorkflowResponse>>
{
    public async Task<Result<AssessmentWorkflowResponse>> Handle(
        GetAssessmentWorkflowQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
        {
            return Result<AssessmentWorkflowResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentWorkflow.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 3. Load assessment session with tenant isolation
        //
        // AssessmentSession does not contain OrganizationId.
        // Therefore scope through CandidateApplication.
        // ---------------------------------------------------------

        var session = await (
            from assessmentSession in context
                .Set<AssessmentSession>()
                .AsNoTracking()

            join candidateApplication in context
                .Set<CandidateApplication>()
                .AsNoTracking()
                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentSession.Id == request.AssessmentSessionId
                  && candidateApplication.IdentityUserId == currentUser.UserId.Value

            select assessmentSession
        ).FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result<AssessmentWorkflowResponse>.Failure(
                Error.NotFound(
                    "AssessmentSession.NotFound",
                    "The assessment session could not be found."));
        }

        // ---------------------------------------------------------
        // 4. Load session rounds and questions
        //
        // We only need status/order/progress information.
        // The full question payload is intentionally not returned.
        // ---------------------------------------------------------

        var rounds = await context
            .Set<AssessmentSessionRound>()
            .AsNoTracking()
            .Where(x =>
                x.AssessmentSessionId == session.Id)
            .Include(x => x.Questions)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // 5. Calculate round workflow
        // ---------------------------------------------------------

        var roundResponses = rounds
            .Select(CreateRoundResponse)
            .ToList();

        // ---------------------------------------------------------
        // 6. Calculate overall round counts
        // ---------------------------------------------------------

        var totalRounds = rounds.Count;

        var completedRounds = rounds.Count(
            x => x.Status ==
                 AssessmentSessionRoundStatus.Completed);

        var remainingRounds = rounds.Count(
            x =>
                x.Status ==
                    AssessmentSessionRoundStatus.InProgress
                || x.Status ==
                    AssessmentSessionRoundStatus.NotStarted);

        // ---------------------------------------------------------
        // 7. Calculate overall question counts
        // ---------------------------------------------------------

        var totalQuestions = rounds.Sum(
            x => x.TargetQuestionCount);

        var completedQuestions = rounds
            .SelectMany(x => x.Questions)
            .Count(
                x =>
                    x.Status ==
                        AssessmentQuestionStatus.Completed);

        var skippedQuestions = rounds
            .SelectMany(x => x.Questions)
            .Count(
                x =>
                    x.Status ==
                        AssessmentQuestionStatus.Skipped);

        var remainingQuestions = Math.Max(
            0,
            totalQuestions
            - completedQuestions
            - skippedQuestions);

        // ---------------------------------------------------------
        // 8. Calculate overall completion percentage
        //
        // Completed + skipped questions count as progress.
        // ---------------------------------------------------------

        var completionPercentage =
            CalculatePercentage(
                completedQuestions + skippedQuestions,
                totalQuestions);

        // ---------------------------------------------------------
        // 9. Find current round
        // ---------------------------------------------------------

        AssessmentSessionRound? currentRound = null;

        if (session.CurrentSessionRoundId.HasValue)
        {
            currentRound = rounds.FirstOrDefault(
                x =>
                    x.Id ==
                    session.CurrentSessionRoundId.Value);
        }

        // ---------------------------------------------------------
        // 10. Find current question
        // ---------------------------------------------------------

        AssessmentQuestion? currentQuestion = null;

        if (session.CurrentQuestionId.HasValue)
        {
            currentQuestion = rounds
                .SelectMany(x => x.Questions)
                .FirstOrDefault(
                    x =>
                        x.Id ==
                        session.CurrentQuestionId.Value);
        }

        // ---------------------------------------------------------
        // 11. Determine current stage
        // ---------------------------------------------------------

        var currentStage = DetermineCurrentStage(
            session,
            currentRound);

        // ---------------------------------------------------------
        // 12. Determine whether candidate can resume
        // ---------------------------------------------------------

        var canResume =
            session.Status ==
                AssessmentSessionStatus.InProgress
            && !session.IsExpired();

        // ---------------------------------------------------------
        // 13. Build response
        // ---------------------------------------------------------

        var response = new AssessmentWorkflowResponse(
            AssessmentSessionId:
                session.Id,

            Status:
                session.Status.ToString(),

            CurrentStage:
                currentStage,

            CompletionPercentage:
                completionPercentage,

            TotalRounds:
                totalRounds,

            CompletedRounds:
                completedRounds,

            RemainingRounds:
                remainingRounds,

            TotalQuestions:
                totalQuestions,

            CompletedQuestions:
                completedQuestions,

            SkippedQuestions:
                skippedQuestions,

            RemainingQuestions:
                remainingQuestions,

            CurrentRoundId:
                currentRound?.Id,

            CurrentRoundOrder:
                currentRound?.Order,

            CurrentRoundType:
                currentRound?.Type.ToString(),

            CurrentQuestionId:
                currentQuestion?.Id,

            CurrentQuestionOrder:
                currentQuestion?.Order,

            StartedOn:
                session.StartedOn,

            LastActivityOn:
                session.LastActivityOn,

            AssessmentExpiresOn:
                session.AssessmentExpiresOn,

            CanResume:
                canResume,

            IsInterrupted:
                session.IsInterrupted,

            Rounds:
                roundResponses);

        return Result<AssessmentWorkflowResponse>.Success(
            response);
    }

    private static AssessmentWorkflowRoundResponse
        CreateRoundResponse(
            AssessmentSessionRound round)
    {
        var completedQuestions = round.Questions.Count(
            x =>
                x.Status ==
                AssessmentQuestionStatus.Completed);

        var skippedQuestions = round.Questions.Count(
            x =>
                x.Status ==
                AssessmentQuestionStatus.Skipped);

        var remainingQuestions = Math.Max(
            0,
            round.TargetQuestionCount
            - completedQuestions
            - skippedQuestions);

        var completionPercentage =
            CalculatePercentage(
                completedQuestions + skippedQuestions,
                round.TargetQuestionCount);

        return new AssessmentWorkflowRoundResponse(
            RoundId:
                round.Id,

            Order:
                round.Order,

            Type:
                round.Type.ToString(),

            Status:
                round.Status.ToString(),

            TotalQuestions:
                round.TargetQuestionCount,

            CompletedQuestions:
                completedQuestions,

            SkippedQuestions:
                skippedQuestions,

            RemainingQuestions:
                remainingQuestions,

            CompletionPercentage:
                completionPercentage,

            DurationInMinutes:
                round.DurationInMinutes,

            StartedOn:
                round.StartedOn,

            CompletedOn:
                round.CompletedOn);
    }

    private static string DetermineCurrentStage(
        AssessmentSession session,
        AssessmentSessionRound? currentRound)
    {
        return session.Status switch
        {
            AssessmentSessionStatus.NotStarted =>
                "Assessment Ready",

            AssessmentSessionStatus.Completed =>
                "Assessment Completed",

            AssessmentSessionStatus.Abandoned =>
                "Assessment Abandoned",

            AssessmentSessionStatus.Expired =>
                "Assessment Expired",

            AssessmentSessionStatus.Terminated =>
                "Assessment Terminated",

            AssessmentSessionStatus.InProgress
                when session.IsInterrupted =>
                "Assessment Interrupted",

            AssessmentSessionStatus.InProgress
                when currentRound is not null =>
                $"Round {currentRound.Order} - " +
                $"{currentRound.Type}",

            AssessmentSessionStatus.InProgress =>
                "Assessment In Progress",

            _ =>
                "Assessment In Progress"
        };
    }

    private static int CalculatePercentage(
        int completed,
        int total)
    {
        if (total <= 0)
        {
            return 0;
        }

        return (completed * 100) / total;
    }
}