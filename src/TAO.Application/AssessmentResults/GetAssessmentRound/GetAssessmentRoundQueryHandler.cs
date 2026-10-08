using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentRound;

internal sealed class GetAssessmentRoundQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetAssessmentRoundQuery,
        Result<GetAssessmentRoundResponse>>
{
    public async Task<Result<GetAssessmentRoundResponse>> Handle(
        GetAssessmentRoundQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Authentication + organization validation
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null ||
            currentUser.OrganizationId is null)
        {
            return Result<GetAssessmentRoundResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResult.Unauthorized",
                    "The current user is not authorized to view assessment results."));
        }

        // ---------------------------------------------------------
        // 2. Recruiter / Hiring Manager only
        // ---------------------------------------------------------

        if (currentUser.Role is not
            (UserRole.Recruiter or UserRole.HiringManager))
        {
            return Result<GetAssessmentRoundResponse>.Failure(
                Error.Forbidden(
                    "AssessmentResult.Forbidden",
                    "The current user does not have permission to view assessment results."));
        }

        var organizationId = currentUser.OrganizationId.Value;

        // ---------------------------------------------------------
        // 3. Validate session + round belong to same organization
        // ---------------------------------------------------------

        var round = await (
            from assessmentSessionRound in context
                .Set<AssessmentSessionRound>()

            join assessmentSession in context
                .Set<AssessmentSession>()
                on assessmentSessionRound.AssessmentSessionId
                    equals assessmentSession.Id

            join candidateApplication in context
                .Set<CandidateApplication>()
                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentSessionRound.Id == request.RoundId
                  && assessmentSession.Id == request.AssessmentSessionId
                  && candidateApplication.OrganizationId == organizationId

            select assessmentSessionRound
        )
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

        if (round is null)
        {
            return Result<GetAssessmentRoundResponse>.Failure(
                Error.NotFound(
                    "AssessmentResult.RoundNotFound",
                    $"Assessment round '{request.RoundId}' was not found."));
        }

        // ---------------------------------------------------------
        // 4. Load round evaluation
        // ---------------------------------------------------------

        var roundEvaluation = await context
            .Set<AssessmentRoundEvaluation>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.AssessmentSessionRoundId == round.Id,
                cancellationToken);

        if (roundEvaluation is null)
        {
            return Result<GetAssessmentRoundResponse>.Failure(
                Error.NotFound(
                    "AssessmentResult.RoundEvaluationNotFound",
                    "The assessment round evaluation has not been generated yet."));
        }

        // ---------------------------------------------------------
        // 5. Load questions
        // ---------------------------------------------------------

        var questions = await context
            .Set<AssessmentQuestion>()
            .AsNoTracking()
            .Where(x =>
                x.AssessmentSessionRoundId == round.Id)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // 6. Load question evaluations
        // ---------------------------------------------------------

        var questionIds = questions
            .Select(x => x.Id)
            .ToList();

        var questionEvaluations = await context
            .Set<AssessmentQuestionEvaluation>()
            .AsNoTracking()
            .Where(x =>
                questionIds.Contains(x.AssessmentQuestionId))
            .ToDictionaryAsync(
                x => x.AssessmentQuestionId,
                cancellationToken);

        // ---------------------------------------------------------
        // 7. Build question summaries
        // ---------------------------------------------------------

        var questionSummaries =
            new List<AssessmentQuestionSummary>(
                questions.Count);

        foreach (var question in questions)
        {
            questionEvaluations.TryGetValue(
                question.Id,
                out var evaluation);

            var strengths = evaluation?.Strengths
                ?? Array.Empty<string>();

            var gaps = evaluation?.Gaps
                ?? Array.Empty<string>();

            var evidence = evaluation?.Evidence
                ?? Array.Empty<string>();

            var competencies = evaluation?.Competencies
                .Select(
                    competency =>
                        new AssessmentQuestionCompetencySummary(
                            competency.Name,
                            competency.Score))
                .ToList()
                ?? [];

            questionSummaries.Add(
                new AssessmentQuestionSummary(
                    question.Id,
                    question.Order,
                    question.PrimaryQuestion,
                    question.Status.ToString(),
                    evaluation?.Score,
                    evaluation?.Confidence,
                    strengths,
                    gaps,
                    evidence,
                    competencies,
                    HasConversation:
                        question.Conversation is not null,
                    HasCandidateCode:
                        !string.IsNullOrWhiteSpace(
                            question.CandidateCode)));
        }

        // ---------------------------------------------------------
        // 8. Build response
        // ---------------------------------------------------------

        var response = new GetAssessmentRoundResponse(
            request.AssessmentSessionId,
            round.Id,
            round.Order,
            round.Type.ToString(),
            roundEvaluation.Score,
            roundEvaluation.Confidence,
            roundEvaluation.Strengths,
            roundEvaluation.Gaps,
            roundEvaluation.Evidence,
            questionSummaries);

        return Result<GetAssessmentRoundResponse>.Success(
            response);
    }
}