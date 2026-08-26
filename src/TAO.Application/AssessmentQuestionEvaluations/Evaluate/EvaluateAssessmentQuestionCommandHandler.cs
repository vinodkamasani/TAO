using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.AssessmentQuestionEvaluations;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestionEvaluations.Evaluate;

internal sealed class EvaluateAssessmentQuestionCommandHandler
    : IRequestHandler<EvaluateAssessmentQuestionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IAssessmentQuestionEvaluationGenerator _generator;

    public EvaluateAssessmentQuestionCommandHandler(
        IApplicationDbContext context,
        IAssessmentQuestionEvaluationGenerator generator)
    {
        _context = context;
        _generator = generator;
    }

    public async Task<Result> Handle(
        EvaluateAssessmentQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var question = await _context
            .Set<AssessmentQuestion>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentQuestionId,
                cancellationToken);

        if (question is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.AssessmentQuestionId}' was not found."));
        }

        if (question.Status != AssessmentQuestionStatus.Completed)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.NotCompleted",
                    "Only a completed assessment question can be evaluated."));
        }

        var sessionRound = await _context
            .Set<AssessmentSessionRound>()
            .FirstOrDefaultAsync(
                x => x.Id == question.AssessmentSessionRoundId,
                cancellationToken);

        if (sessionRound is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    $"Assessment session round '{question.AssessmentSessionRoundId}' was not found."));
        }

        var existingEvaluation = await _context
            .Set<AssessmentQuestionEvaluation>()
            .FirstOrDefaultAsync(
                x => x.AssessmentQuestionId == question.Id,
                cancellationToken);

        if (existingEvaluation is not null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.AlreadyExists",
                    "The assessment question has already been evaluated."));
        }

        var generationResult = await _generator.GenerateAsync(
            question,
            sessionRound,
            cancellationToken);

        if (generationResult.IsFailure)
        {
            return Result.Failure(
                generationResult.Error!);
        }

        var evaluation = AssessmentQuestionEvaluation.Create(
            question.Id,
            generationResult.Value!.Score,
            generationResult.Value.Confidence,
            generationResult.Value.Strengths,
            generationResult.Value.Gaps,
            generationResult.Value.Evidence,
            generationResult.Value.Competencies);


        _context
            .Set<AssessmentQuestionEvaluation>()
            .Add(evaluation);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}