using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentSummary;

internal sealed class GetAssessmentSummaryQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetAssessmentSummaryQuery,
        Result<GetAssessmentSummaryResponse>>
{
    public async Task<Result<GetAssessmentSummaryResponse>> Handle(
        GetAssessmentSummaryQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication and organization
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null ||
            currentUser.OrganizationId is null)
        {
            return Result<GetAssessmentSummaryResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResult.Unauthorized",
                    "The current user is not authorized to view assessment results."));
        }

        // ---------------------------------------------------------
        // 2. Validate role
        //
        // Assessment results are recruiter-facing.
        // Hiring Managers can also view them.
        // Candidates must never reach this API.
        // ---------------------------------------------------------

        if (currentUser.Role is not
            (UserRole.Recruiter or UserRole.HiringManager))
        {
            return Result<GetAssessmentSummaryResponse>.Failure(
                Error.Forbidden(
                    "AssessmentResult.Forbidden",
                    "The current user does not have permission to view assessment results."));
        }

        var organizationId = currentUser.OrganizationId.Value;

        // ---------------------------------------------------------
        // 3. Load assessment session with organization isolation
        //
        // Never trust OrganizationId from the request.
        // The organization comes from the authenticated user.
        // ---------------------------------------------------------

        var session = await (
            from assessmentSession in context
                .Set<AssessmentSession>()

            join candidateApplication in context
                .Set<CandidateApplication>()
                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentSession.Id == request.AssessmentSessionId
                  && candidateApplication.OrganizationId == organizationId

            select assessmentSession
        ).FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result<GetAssessmentSummaryResponse>.Failure(
                Error.NotFound(
                    "AssessmentResult.NotFound",
                    $"Assessment session '{request.AssessmentSessionId}' was not found."));
        }

        // ---------------------------------------------------------
        // 4. Load assessment result
        // ---------------------------------------------------------

        var assessmentResult = await context
            .Set<AssessmentResult>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.AssessmentSessionId == session.Id,
                cancellationToken);

        if (assessmentResult is null)
        {
            return Result<GetAssessmentSummaryResponse>.Failure(
                Error.NotFound(
                    "AssessmentResult.NotFound",
                    "The assessment result has not been generated yet."));
        }

        // ---------------------------------------------------------
        // 5. Load competency evaluations
        // ---------------------------------------------------------

        var competencies = await context
            .Set<AssessmentCompetencyEvaluation>()
            .AsNoTracking()
            .Where(x =>
                x.AssessmentResultId == assessmentResult.Id)
            .OrderBy(x => x.CompetencyName)
            .Select(x =>
                new AssessmentCompetencySummary(
                    x.CompetencyName,
                    x.Priority,
                    x.Score,
                    x.MinimumPassPercentage,
                    x.IsPassed))
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // 6. Load assessment rounds
        // ---------------------------------------------------------

        var rounds = await context
            .Set<AssessmentSessionRound>()
            .AsNoTracking()
            .Where(x =>
                x.AssessmentSessionId == session.Id)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        if (rounds.Count == 0)
        {
            return Result<GetAssessmentSummaryResponse>.Failure(
                Error.Validation(
                    "AssessmentResult.NoRounds",
                    "The assessment session does not contain any rounds."));
        }

        var roundIds = rounds
            .Select(x => x.Id)
            .ToList();

        // ---------------------------------------------------------
        // 7. Load round evaluations
        // ---------------------------------------------------------

        var roundEvaluations = await context
            .Set<AssessmentRoundEvaluation>()
            .AsNoTracking()
            .Where(x =>
                roundIds.Contains(x.AssessmentSessionRoundId))
            .ToDictionaryAsync(
                x => x.AssessmentSessionRoundId,
                cancellationToken);

        // ---------------------------------------------------------
        // 8. Load question counts
        // ---------------------------------------------------------

        var questionCounts = await context
            .Set<AssessmentQuestion>()
            .AsNoTracking()
            .Where(x =>
                roundIds.Contains(x.AssessmentSessionRoundId))
            .GroupBy(x => x.AssessmentSessionRoundId)
            .Select(group => new
            {
                RoundId = group.Key,

                TotalQuestions = group.Count(),

                CompletedQuestions = group.Count(
                    x =>
                        x.Status ==
                        AssessmentQuestionStatus.Completed),

                SkippedQuestions = group.Count(
                    x =>
                        x.Status ==
                        AssessmentQuestionStatus.Skipped)
            })
            .ToDictionaryAsync(
                x => x.RoundId,
                cancellationToken);

        // ---------------------------------------------------------
        // 9. Build round summaries
        // ---------------------------------------------------------

        var roundSummaries =
            new List<AssessmentRoundSummary>(
                rounds.Count);

        foreach (var round in rounds)
        {
            if (!roundEvaluations.TryGetValue(
                    round.Id,
                    out var roundEvaluation))
            {
                continue;
            }

            questionCounts.TryGetValue(
                round.Id,
                out var questionCount);

            roundSummaries.Add(
                new AssessmentRoundSummary(
                    round.Id,
                    round.Order,
                    round.Type.ToString(),
                    roundEvaluation.Score,
                    roundEvaluation.Confidence,
                    questionCount?.TotalQuestions ?? 0,
                    questionCount?.CompletedQuestions ?? 0,
                    questionCount?.SkippedQuestions ?? 0));
        }

        // ---------------------------------------------------------
        // 10. Build response
        // ---------------------------------------------------------

        var response = new GetAssessmentSummaryResponse(
            session.Id,
            assessmentResult.Id,
            assessmentResult.OverallScore,
            assessmentResult.OverallConfidence,
            assessmentResult.Recommendation.ToString(),
            assessmentResult.ExecutiveSummary,
            competencies,
            roundSummaries);

        return Result<GetAssessmentSummaryResponse>.Success(
            response);
    }
}