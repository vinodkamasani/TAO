using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentEvaluations.Evaluate;
using TAO.Application.AssessmentQuestions.Services;
using TAO.Application.AssessmentRoundEvaluations.Evaluate;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Advance;

internal sealed class AdvanceAssessmentSessionCommandHandler
    : IRequestHandler<
        AdvanceAssessmentSessionCommand,
        Result<AdvanceAssessmentSessionResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAssessmentQuestionGenerationService _questionGenerationService;
    private readonly ISender _sender;
    private readonly ICurrentUser _currentUser;

    public AdvanceAssessmentSessionCommandHandler(
        IApplicationDbContext context,
        IAssessmentQuestionGenerationService questionGenerationService,
        ISender sender,
        ICurrentUser currentUser)
    {
        _context = context;
        _questionGenerationService = questionGenerationService;
        _sender = sender;
        _currentUser = currentUser;
    }

    public async Task<Result<AdvanceAssessmentSessionResponse>> Handle(
        AdvanceAssessmentSessionCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentSession.Unauthorized",
                    "The current user is not authenticated."));
        }

    
        // ---------------------------------------------------------
        // 3. Load assessment session with tenant isolation
        // ---------------------------------------------------------

        var session = await (
            from assessmentSession in _context
                .Set<AssessmentSession>()

            join candidateApplication in _context
                .Set<CandidateApplication>()

                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentSession.Id == request.AssessmentSessionId
                  && candidateApplication.IdentityUserId
                      == _currentUser.UserId.Value

            select assessmentSession
        ).FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.NotFound(
                    "AssessmentSession.NotFound",
                    $"Assessment session '{request.AssessmentSessionId}' was not found."));
        }

        // ---------------------------------------------------------
        // 4. Validate session status
        // ---------------------------------------------------------

        if (session.Status != AssessmentSessionStatus.InProgress)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentSession.NotInProgress",
                    "Only an in-progress assessment session can be advanced."));
        }

        // ---------------------------------------------------------
        // 5. Validate current round
        // ---------------------------------------------------------

        if (!session.CurrentSessionRoundId.HasValue)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentSession.NoCurrentRound",
                    "The assessment session does not have a current round."));
        }

        // ---------------------------------------------------------
        // 6. Load current round with tenant isolation
        //
        // The session itself has already been tenant-scoped, and
        // this round must belong to that session.
        // ---------------------------------------------------------

        var currentRound = await _context
            .Set<AssessmentSessionRound>()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == session.CurrentSessionRoundId.Value &&
                    x.AssessmentSessionId == session.Id,
                cancellationToken);

        if (currentRound is null)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    "The current assessment session round was not found."));
        }

        // ---------------------------------------------------------
        // 7. Validate current round status
        // ---------------------------------------------------------

        if (currentRound.Status !=
            AssessmentSessionRoundStatus.InProgress)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentSessionRound.NotInProgress",
                    "The current assessment session round is not in progress."));
        }

        // ---------------------------------------------------------
        // 8. Count completed/skipped questions
        // ---------------------------------------------------------

        var completedQuestionCount = await _context
            .Set<AssessmentQuestion>()
            .CountAsync(
                x =>
                    x.AssessmentSessionRoundId ==
                        currentRound.Id
                    &&
                    (
                        x.Status ==
                            AssessmentQuestionStatus.Completed
                        ||
                        x.Status ==
                            AssessmentQuestionStatus.Skipped
                    ),
                cancellationToken);

        // ---------------------------------------------------------
        // 9. Current round still has questions remaining
        // ---------------------------------------------------------

        if (completedQuestionCount <
            currentRound.TargetQuestionCount)
        {
            var generationResult =
                await _questionGenerationService.GenerateNextAsync(
                    session,
                    currentRound,
                    cancellationToken);

            if (generationResult.IsFailure)
            {
                return Result<AdvanceAssessmentSessionResponse>.Failure(
                    generationResult.Error!);
            }

            var nextQuestion = generationResult.Value!;

            session.SetCurrentQuestion(
                nextQuestion.Id);

            _context
                .Set<AssessmentQuestion>()
                .Add(nextQuestion);

            await _context.SaveChangesAsync(
                cancellationToken);

            return Result<AdvanceAssessmentSessionResponse>.Success(
                CreateResponse(
                    nextQuestion,
                    currentRound,
                    isNewRound: false,
                    assessmentCompleted: false));
        }

        // ---------------------------------------------------------
        // 10. Current round is complete.
        //
        // Evaluate the round before marking it completed.
        // ---------------------------------------------------------

        var evaluationResult = await _sender.Send(
            new EvaluateAssessmentRoundCommand(
                currentRound.Id),
            cancellationToken);

        if (evaluationResult.IsFailure)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                evaluationResult.Error!);
        }

        // ---------------------------------------------------------
        // 11. Mark current round completed
        // ---------------------------------------------------------

        currentRound.Complete(
            DateTime.UtcNow);

        // ---------------------------------------------------------
        // 12. Find next round
        // ---------------------------------------------------------

        var nextRound = await _context
            .Set<AssessmentSessionRound>()
            .Where(
                x =>
                    x.AssessmentSessionId == session.Id
                    &&
                    x.Order > currentRound.Order
                    &&
                    x.Status ==
                        AssessmentSessionRoundStatus.NotStarted)
            .OrderBy(x => x.Order)
            .FirstOrDefaultAsync(
                cancellationToken);

        // ---------------------------------------------------------
        // 13. No more rounds
        // ---------------------------------------------------------

        if (nextRound is null)
        {
            var finalEvaluationResult = await _sender.Send(
                new EvaluateAssessmentCommand(
                    session.Id),
                cancellationToken);

            if (finalEvaluationResult.IsFailure)
            {
                return Result<AdvanceAssessmentSessionResponse>.Failure(
                    finalEvaluationResult.Error!);
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            return Result<AdvanceAssessmentSessionResponse>.Success(
                new AdvanceAssessmentSessionResponse(
                    QuestionId: null,
                    QuestionOrder: null,
                    PrimaryQuestion: null,
                    Competencies: Array.Empty<string>(),
                    RoundId: null,
                    RoundOrder: null,
                    RoundType: null,
                    RoundDurationInMinutes: null,
                    IsNewRound: false,
                    AssessmentCompleted: true));
        }

        // ---------------------------------------------------------
        // 14. Start next round
        // ---------------------------------------------------------

        var startedOn = DateTime.UtcNow;

        nextRound.Start(
            startedOn,
            startedOn.AddMinutes(
                nextRound.DurationInMinutes));

        session.SetCurrentRound(
            nextRound.Id);

        // ---------------------------------------------------------
        // 15. Generate first question for next round
        // ---------------------------------------------------------

        var generationResultForNextRound =
            await _questionGenerationService.GenerateNextAsync(
                session,
                nextRound,
                cancellationToken);

        if (generationResultForNextRound.IsFailure)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                generationResultForNextRound.Error!);
        }

        var firstQuestion =
            generationResultForNextRound.Value!;

        // ---------------------------------------------------------
        // 16. Set current question
        // ---------------------------------------------------------

        session.SetCurrentQuestion(
            firstQuestion.Id);

        _context
            .Set<AssessmentQuestion>()
            .Add(firstQuestion);

        // ---------------------------------------------------------
        // 17. Persist everything
        // ---------------------------------------------------------

        await _context.SaveChangesAsync(
            cancellationToken);

        // ---------------------------------------------------------
        // 18. Return first question of new round
        // ---------------------------------------------------------

        return Result<AdvanceAssessmentSessionResponse>.Success(
            CreateResponse(
                firstQuestion,
                nextRound,
                isNewRound: true,
                assessmentCompleted: false));
    }

    private static AdvanceAssessmentSessionResponse CreateResponse(
        AssessmentQuestion question,
        AssessmentSessionRound round,
        bool isNewRound,
        bool assessmentCompleted)
    {
        return new AdvanceAssessmentSessionResponse(
            QuestionId: question.Id,
            QuestionOrder: question.Order,
            PrimaryQuestion: question.PrimaryQuestion,
            Competencies: question.Competencies,
            RoundId: round.Id,
            RoundOrder: round.Order,
            RoundType: round.Type.ToString(),
            RoundDurationInMinutes: round.DurationInMinutes,
            IsNewRound: isNewRound,
            AssessmentCompleted: assessmentCompleted);
    }
}