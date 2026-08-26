using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentEvaluations.Evaluate;

internal sealed class EvaluateAssessmentCommandHandler
    : IRequestHandler<EvaluateAssessmentCommand, Result>
{
    private const byte OverallPassPercentage = 70;

    private readonly IApplicationDbContext _context;
    private readonly IAssessmentEvaluationGenerator _generator;

    public EvaluateAssessmentCommandHandler(
        IApplicationDbContext context,
        IAssessmentEvaluationGenerator generator)
    {
        _context = context;
        _generator = generator;
    }

    public async Task<Result> Handle(
        EvaluateAssessmentCommand request,
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

        var existingResult = await _context
            .Set<AssessmentResult>()
            .FirstOrDefaultAsync(
                x => x.AssessmentSessionId == session.Id,
                cancellationToken);

        if (existingResult is not null)
        {
            return Result.Success();
        }

        var sessionRounds = await _context
            .Set<AssessmentSessionRound>()
            .Where(x => x.AssessmentSessionId == session.Id)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        if (sessionRounds.Count == 0)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentEvaluation.NoRounds",
                    "The assessment session does not contain any rounds."));
        }

        var roundIds = sessionRounds
            .Select(x => x.Id)
            .ToList();

        var roundEvaluations = await _context
            .Set<AssessmentRoundEvaluation>()
            .Where(x => roundIds.Contains(x.AssessmentSessionRoundId))
            .ToListAsync(cancellationToken);

        if (roundEvaluations.Count != sessionRounds.Count)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentEvaluation.RoundEvaluationsIncomplete",
                    "All assessment rounds must have an evaluation before the assessment can be evaluated."));
        }

        var generationResult = await _generator.GenerateAsync(
            session,
            roundEvaluations,
            cancellationToken);

        if (generationResult.IsFailure)
        {
            return Result.Failure(
                generationResult.Error!);
        }

        var generated = generationResult.Value!;

        var overallScore = CalculateOverallScore(
            roundEvaluations);

        var competencyDefinitions = sessionRounds
            .SelectMany(x => x.Competencies)
            .GroupBy(
                x => x.Name,
                StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .ToList();

        var competencyScores =
            CalculateCompetencyScores(
                sessionRounds,
                roundEvaluations,
                competencyDefinitions);

        var recommendation =
            DetermineRecommendation(
                overallScore,
                competencyScores);

        var assessmentResult = AssessmentResult.Create(
            session.Id,
            overallScore,
            generated.Confidence,
            recommendation,
            generated.ExecutiveSummary);

        _context
            .Set<AssessmentResult>()
            .Add(assessmentResult);

        foreach (var competency in competencyScores)
        {
            var competencyEvaluation =
                AssessmentCompetencyEvaluation.Create(
                    assessmentResult.Id,
                    competency.Name,
                    competency.Priority,
                    competency.Score,
                    competency.MinimumPassPercentage);

            _context
                .Set<AssessmentCompetencyEvaluation>()
                .Add(competencyEvaluation);
        }

        session.Complete();

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    private static byte CalculateOverallScore(
        IReadOnlyCollection<AssessmentRoundEvaluation> evaluations)
    {
        return (byte)Math.Round(
            evaluations.Average(x => x.Score),
            MidpointRounding.AwayFromZero);
    }

    private static IReadOnlyCollection<CompetencyScore> CalculateCompetencyScores(
        IReadOnlyCollection<AssessmentSessionRound> rounds,
        IReadOnlyCollection<AssessmentRoundEvaluation> evaluations,
        IReadOnlyCollection<
            TAO.Domain.ValueObjects.AssessmentRoundCompetency> competencies)
    {
        var result = new List<CompetencyScore>();

        foreach (var competency in competencies)
        {
            var relevantRoundIds = rounds
                .Where(round =>
                    round.Competencies.Any(
                        x => string.Equals(
                            x.Name,
                            competency.Name,
                            StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.Id)
                .ToHashSet();

            var relevantEvaluations = evaluations
                .Where(x =>
                    relevantRoundIds.Contains(
                        x.AssessmentSessionRoundId))
                .ToList();

            if (relevantEvaluations.Count == 0)
            {
                continue;
            }

            var score = (byte)Math.Round(
                relevantEvaluations.Average(x => x.Score),
                MidpointRounding.AwayFromZero);

            result.Add(
                new CompetencyScore(
                    competency.Name,
                    competency.Priority,
                    score,
                    competency.MinimumPassPercentage));
        }

        return result;
    }

    private static AssessmentRecommendation DetermineRecommendation(
        byte overallScore,
        IReadOnlyCollection<CompetencyScore> competencyScores)
    {
        if (overallScore < OverallPassPercentage)
        {
            return AssessmentRecommendation.NotRecommended;
        }

        var requiredCompetenciesPassed =
            competencyScores
                .Where(x =>
                    string.Equals(
                        x.Priority,
                        "Required",
                        StringComparison.OrdinalIgnoreCase))
                .All(x =>
                    x.Score >= x.MinimumPassPercentage);

        return requiredCompetenciesPassed
            ? AssessmentRecommendation.Recommended
            : AssessmentRecommendation.NotRecommended;
    }

    private sealed record CompetencyScore(
        string Name,
        string Priority,
        byte Score,
        byte MinimumPassPercentage);
}