using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentEvaluations.Evaluate;
using TAO.Application.AssessmentQuestions.Services;
using TAO.Application.AssessmentRoundEvaluations.Evaluate;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Advance;

internal sealed class AdvanceAssessmentSessionCommandHandler
    : IRequestHandler<AdvanceAssessmentSessionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IAssessmentQuestionGenerationService _questionGenerationService;
    private readonly ISender _sender;

    public AdvanceAssessmentSessionCommandHandler(
        IApplicationDbContext context,
        IAssessmentQuestionGenerationService questionGenerationService,
        ISender sender)
    {
        _context = context;
        _questionGenerationService = questionGenerationService;
        _sender = sender;
    }

    public async Task<Result> Handle(
        AdvanceAssessmentSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _context
            .Set<AssessmentSession>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentSessionId,
                cancellationToken);

        if (session is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentSession.NotFound",
                    $"Assessment session '{request.AssessmentSessionId}' was not found."));
        }

        if (session.Status != AssessmentSessionStatus.InProgress)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentSession.NotInProgress",
                    "Only an in-progress assessment session can be advanced."));
        }

        if (!session.CurrentSessionRoundId.HasValue)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentSession.NoCurrentRound",
                    "The assessment session does not have a current round."));
        }

        var currentRound = await _context
            .Set<AssessmentSessionRound>()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == session.CurrentSessionRoundId.Value &&
                    x.AssessmentSessionId == session.Id,
                cancellationToken);

        if (currentRound is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    "The current assessment session round was not found."));
        }

        if (currentRound.Status != AssessmentSessionRoundStatus.InProgress)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentSessionRound.NotInProgress",
                    "The current assessment session round is not in progress."));
        }

        var completedQuestionCount = await _context
            .Set<AssessmentQuestion>()
            .CountAsync(
                x =>
                    x.AssessmentSessionRoundId == currentRound.Id &&
                    (x.Status == AssessmentQuestionStatus.Completed ||
                     x.Status == AssessmentQuestionStatus.Skipped),
                cancellationToken);

        // There are still questions remaining in the current round.
        if (completedQuestionCount < currentRound.TargetQuestionCount)
        {
            var generationResult =
                await _questionGenerationService.GenerateNextAsync(
                    session,
                    currentRound,
                    cancellationToken);

            if (generationResult.IsFailure)
            {
                return Result.Failure(
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

            return Result.Success();
        }

        // Current round is complete.
        // Evaluate it before marking the round as completed.
        var evaluationResult = await _sender.Send(
            new EvaluateAssessmentRoundCommand(
                currentRound.Id),
            cancellationToken);

        if (evaluationResult.IsFailure)
        {
            return Result.Failure(
                evaluationResult.Error!);
        }

        currentRound.Complete(
            DateTime.UtcNow);

        var nextRound = await _context
            .Set<AssessmentSessionRound>()
            .Where(
                x =>
                    x.AssessmentSessionId == session.Id &&
                    x.Order > currentRound.Order &&
                    x.Status == AssessmentSessionRoundStatus.NotStarted)
            .OrderBy(x => x.Order)
            .FirstOrDefaultAsync(
                cancellationToken);

        if (nextRound is null)
        {
            var finalEvaluationResult = await _sender.Send(
         new EvaluateAssessmentCommand(
             session.Id),
         cancellationToken);

            if (finalEvaluationResult.IsFailure)
            {
                return Result.Failure(
                    finalEvaluationResult.Error!);
            }

            return Result.Success();
        }

        var startedOn = DateTime.UtcNow;

        nextRound.Start(
            startedOn,
            startedOn.AddMinutes(
                nextRound.DurationInMinutes));

        session.SetCurrentRound(
            nextRound.Id);

        var generationResultForNextRound =
            await _questionGenerationService.GenerateNextAsync(
                session,
                nextRound,
                cancellationToken);

        if (generationResultForNextRound.IsFailure)
        {
            return Result.Failure(
                generationResultForNextRound.Error!);
        }

        var firstQuestion = generationResultForNextRound.Value!;

        session.SetCurrentQuestion(
            firstQuestion.Id);

        _context
            .Set<AssessmentQuestion>()
            .Add(firstQuestion);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}