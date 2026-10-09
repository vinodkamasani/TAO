using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentQuestion;

internal sealed class GetAssessmentQuestionQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetAssessmentQuestionQuery,
        Result<GetAssessmentQuestionResponse>>
{
    public async Task<Result<GetAssessmentQuestionResponse>> Handle(
        GetAssessmentQuestionQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Authentication + organization validation
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null ||
            currentUser.OrganizationId is null)
        {
            return Result<GetAssessmentQuestionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResult.Unauthorized",
                    "The current user is not authorized to view assessment results."));
        }

        // ---------------------------------------------------------
        // 2. Recruiter / Hiring Manager only
        // ---------------------------------------------------------

        if (currentUser.Role is not
            (UserRole.Recruiter or UserRole.HiringManager or UserRole.Administrator))
        {
            return Result<GetAssessmentQuestionResponse>.Failure(
                Error.Forbidden(
                    "AssessmentResult.Forbidden",
                    "The current user does not have permission to view assessment results."));
        }

        var organizationId = currentUser.OrganizationId.Value;

        // ---------------------------------------------------------
        // 3. Validate question belongs to the requested session
        //    and organization
        // ---------------------------------------------------------

        var question = await (
            from assessmentQuestion in context
                .Set<AssessmentQuestion>()

            join sessionRound in context
                .Set<AssessmentSessionRound>()
                on assessmentQuestion.AssessmentSessionRoundId
                    equals sessionRound.Id

            join assessmentSession in context
                .Set<AssessmentSession>()
                on sessionRound.AssessmentSessionId
                    equals assessmentSession.Id

            join candidateApplication in context
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
        )
        .AsNoTracking()
        .FirstOrDefaultAsync(cancellationToken);

        if (question is null)
        {
            return Result<GetAssessmentQuestionResponse>.Failure(
                Error.NotFound(
                    "AssessmentResult.QuestionNotFound",
                    $"Assessment question '{request.QuestionId}' was not found."));
        }

        // ---------------------------------------------------------
        // 4. Load question evaluation
        // ---------------------------------------------------------

        var evaluation = await context
            .Set<AssessmentQuestionEvaluation>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.AssessmentQuestionId ==
                    request.QuestionId,
                cancellationToken);

        // ---------------------------------------------------------
        // 5. Build evaluation data
        //
        // A skipped question does not have an evaluation.
        // ---------------------------------------------------------

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
            .ToArray()
            ?? [];

        // ---------------------------------------------------------
        // 6. Build response
        // ---------------------------------------------------------

        var response = new GetAssessmentQuestionResponse(
            request.AssessmentSessionId,
            question.RoundId,
            question.Question.Id,
            question.Question.Order,
            question.Question.PrimaryQuestion,
            question.Question.Status.ToString(),
            evaluation?.Score,
            evaluation?.Confidence,
            strengths,
            gaps,
            evidence,
            competencies,
            HasConversation:
                question.Question.Conversation is not null,
            HasCandidateCode:
                !string.IsNullOrWhiteSpace(
                    question.Question.CandidateCode));

        return Result<GetAssessmentQuestionResponse>.Success(
            response);
    }
}