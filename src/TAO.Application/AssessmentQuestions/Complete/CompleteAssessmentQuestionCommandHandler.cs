using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestionEvaluations.Evaluate;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Complete;

internal sealed class CompleteAssessmentQuestionCommandHandler(
    IApplicationDbContext context,
    ISender sender,
    ICurrentUser currentUser)
    : IRequestHandler<
        CompleteAssessmentQuestionCommand,
        Result<CompleteAssessmentQuestionResponse>>
{
    public async Task<Result<CompleteAssessmentQuestionResponse>> Handle(
        CompleteAssessmentQuestionCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null )
        {
            return Result<CompleteAssessmentQuestionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentQuestion.Unauthorized",
                    "The current user is not authenticated."));
        }

        var userId = currentUser.UserId.Value;

        // ---------------------------------------------------------
        // 2. Load question with tenant isolation
        // ---------------------------------------------------------

        var question = await (
            from q in context.Set<AssessmentQuestion>()

            join sessionRound in context
                .Set<AssessmentSessionRound>()
                on q.AssessmentSessionRoundId
                    equals sessionRound.Id

            join sessionLocal in context
                .Set<AssessmentSession>()
                on sessionRound.AssessmentSessionId
                    equals sessionLocal.Id

            join candidateApplication in context
                .Set<CandidateApplication>()
                on sessionLocal.CandidateApplicationId
                    equals candidateApplication.Id

            where q.Id == request.AssessmentQuestionId
                  && candidateApplication.IdentityUserId == userId

            select q
        ).FirstOrDefaultAsync(cancellationToken);

        if (question is null)
        {
            return Result<CompleteAssessmentQuestionResponse>.Failure(
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
            return Result<CompleteAssessmentQuestionResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.NotCurrentQuestion",
                    "The assessment question is not the current question for an assessment session."));
        }

        try
        {
            // -----------------------------------------------------
            // 4. Complete question
            // -----------------------------------------------------

            question.Complete(DateTime.UtcNow);

            await context.SaveChangesAsync(cancellationToken);

            // -----------------------------------------------------
            // 5. Evaluate completed question
            // -----------------------------------------------------

            var evaluationResult = await sender.Send(
                new EvaluateAssessmentQuestionCommand(
                    question.Id),
                cancellationToken);

            if (evaluationResult.IsFailure)
            {
                return Result<CompleteAssessmentQuestionResponse>.Failure(
                    evaluationResult.Error!);
            }

            // -----------------------------------------------------
            // 6. Do NOT advance here.
            //
            // The UI will explicitly request the next question.
            // -----------------------------------------------------

            return Result<CompleteAssessmentQuestionResponse>.Success(
                new CompleteAssessmentQuestionResponse(
                    question.Id,
                    IsEvaluated: true));
        }
        catch (InvalidOperationException ex)
        {
            return Result<CompleteAssessmentQuestionResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.CannotComplete",
                    ex.Message));
        }
    }
}