using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Skip;

internal sealed class SkipAssessmentQuestionCommandHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser)
    : IRequestHandler<
        SkipAssessmentQuestionCommand,
        Result<SkipAssessmentQuestionResponse>>
{
    public async Task<Result<SkipAssessmentQuestionResponse>> Handle(
        SkipAssessmentQuestionCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null)
        {
            return Result<SkipAssessmentQuestionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentQuestion.Unauthorized",
                    "The current user is not authenticated."));
        }

        var userId = currentUser.UserId.Value;

        // ---------------------------------------------------------
        // 2. Load question belonging to the current candidate
        // ---------------------------------------------------------

        var question = await (
            from q in context.Set<AssessmentQuestion>()

            join sessionRound in context.Set<AssessmentSessionRound>()
                on q.AssessmentSessionRoundId
                    equals sessionRound.Id

            join sessionLocal in context.Set<AssessmentSession>()
                on sessionRound.AssessmentSessionId
                    equals sessionLocal.Id

            join candidateApplication in context.Set<CandidateApplication>()
                on sessionLocal.CandidateApplicationId
                    equals candidateApplication.Id

            where q.Id == request.AssessmentQuestionId
                  && candidateApplication.IdentityUserId == userId

            select q
        ).FirstOrDefaultAsync(cancellationToken);

        if (question is null)
        {
            return Result<SkipAssessmentQuestionResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.AssessmentQuestionId}' was not found."));
        }

        // ---------------------------------------------------------
        // 3. Ensure this is the current question
        // ---------------------------------------------------------

        var session = await (
            from assessmentSession in context
                .Set<AssessmentSession>()

            join candidateApplication in context
                .Set<CandidateApplication>()
                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentSession.CurrentQuestionId == question.Id
                  && assessmentSession.CurrentSessionRoundId ==
                     question.AssessmentSessionRoundId
                  && candidateApplication.IdentityUserId == userId

            select assessmentSession
        ).FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result<SkipAssessmentQuestionResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.NotCurrentQuestion",
                    "The assessment question is not the current question for an assessment session."));
        }

        try
        {
            // -----------------------------------------------------
            // 4. Skip question
            // -----------------------------------------------------

            question.Skip(DateTime.UtcNow);

            await context.SaveChangesAsync(
                cancellationToken);

            // -----------------------------------------------------
            // 5. Do NOT advance here.
            //
            // The UI will explicitly request the next question.
            // -----------------------------------------------------

            return Result<SkipAssessmentQuestionResponse>.Success(
                new SkipAssessmentQuestionResponse(
                    question.Id,
                    IsSkipped: true));
        }
        catch (InvalidOperationException ex)
        {
            return Result<SkipAssessmentQuestionResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.CannotSkip",
                    ex.Message));
        }
    }
}