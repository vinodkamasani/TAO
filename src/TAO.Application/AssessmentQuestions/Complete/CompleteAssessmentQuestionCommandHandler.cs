using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestionEvaluations.Evaluate;
using TAO.Application.AssessmentQuestions.FollowUp;
using TAO.Application.AssessmentSessions.Advance;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Complete;

internal sealed class CompleteAssessmentQuestionCommandHandler
    : IRequestHandler<
        CompleteAssessmentQuestionCommand,
        Result<AdvanceAssessmentSessionResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    private readonly ICurrentUser _currentUser;

    public CompleteAssessmentQuestionCommandHandler(
        IApplicationDbContext context,
        ISender sender,
        ICurrentUser currentUser)
    {
        _context = context;
        _sender = sender;
        _currentUser = currentUser;
    }

    public async Task<Result<AdvanceAssessmentSessionResponse>> Handle(
        CompleteAssessmentQuestionCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null ||
            _currentUser.OrganizationId is null)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentEvaluation.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId =
            _currentUser.OrganizationId.Value;

        // ---------------------------------------------------------
        // 2. Load question with tenant isolation
        //
        // AssessmentQuestion does not have OrganizationId.
        //
        // Question
        //    -> SessionRound
        //    -> Session
        //    -> CandidateApplication
        //    -> Organization
        // ---------------------------------------------------------

        var question = await (
            from q in _context.Set<AssessmentQuestion>()

            join sessionRound in _context
                .Set<AssessmentSessionRound>()
                on q.AssessmentSessionRoundId
                    equals sessionRound.Id

            join sessionLocal in _context
                .Set<AssessmentSession>()
                on sessionRound.AssessmentSessionId
                    equals sessionLocal.Id

            join candidateApplication in _context
                .Set<CandidateApplication>()
                on sessionLocal.CandidateApplicationId
                    equals candidateApplication.Id

            where q.Id == request.AssessmentQuestionId
                  && candidateApplication.OrganizationId
                      == organizationId

            select q
        ).FirstOrDefaultAsync(
            cancellationToken);

        if (question is null)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.AssessmentQuestionId}' was not found."));
        }

        // ---------------------------------------------------------
        // 3. Find the assessment session for which this is the
        //    current question
        //
        // IMPORTANT:
        // Also scope this query through the organization.
        // ---------------------------------------------------------

        var session = await (
            from assessmentSession in _context
                .Set<AssessmentSession>()

            join candidateApplication in _context
                .Set<CandidateApplication>()
                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentSession.CurrentQuestionId
                        == question.Id
                  && assessmentSession.CurrentSessionRoundId
                        == question.AssessmentSessionRoundId
                  && candidateApplication.OrganizationId
                        == organizationId

            select assessmentSession
        ).FirstOrDefaultAsync(
            cancellationToken);

        if (session is null)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.NotCurrentQuestion",
                    "The assessment question is not the current question for an assessment session."));
        }

        // ---------------------------------------------------------
        // 4. Complete question
        // ---------------------------------------------------------

        try
        {
            question.Complete(
                DateTime.UtcNow);

            await _context.SaveChangesAsync(
                cancellationToken);

            // -----------------------------------------------------
            // 5. Evaluate completed question
            // -----------------------------------------------------

            var evaluationResult = await _sender.Send(
                new EvaluateAssessmentQuestionCommand(
                    question.Id),
                cancellationToken);

            if (evaluationResult.IsFailure)
            {
                return Result<AdvanceAssessmentSessionResponse>.Failure(
                    evaluationResult.Error!);
            }

            // -----------------------------------------------------
            // 6. Advance assessment session
            //
            // This now returns the next question, round information,
            // or AssessmentCompleted = true.
            // -----------------------------------------------------

            return await _sender.Send(
                new AdvanceAssessmentSessionCommand(
                    session.Id),
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.CannotComplete",
                    ex.Message));
        }
    }
}