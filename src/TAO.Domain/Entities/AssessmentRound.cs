using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class AssessmentRound : Entity
{
    private readonly List<AssessmentRoundCompetency> _competencies = [];

    private AssessmentRound()
    {
    }

    private AssessmentRound(
        Guid assessmentStrategyId,
        int order,
        AssessmentRoundType type,
        AssessmentDifficulty difficulty,
        int durationInMinutes,
        int targetQuestionCount,
        IReadOnlyCollection<AssessmentRoundCompetency> competencies)
    {
        AssessmentStrategyId = Guard.AgainstEmpty(
            assessmentStrategyId,
            nameof(AssessmentStrategyId));

        if (order <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(order),
                "Round order must be greater than zero.");
        }

        if (durationInMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(durationInMinutes),
                "Round duration must be greater than zero.");
        }

        if (targetQuestionCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(targetQuestionCount),
                "Target question count must be greater than zero.");
        }

        ArgumentNullException.ThrowIfNull(competencies);

        Order = order;
        Type = type;
        Difficulty = difficulty;
        DurationInMinutes = durationInMinutes;
        TargetQuestionCount = targetQuestionCount;

        _competencies.AddRange(competencies);
    }

    public Guid AssessmentStrategyId { get; private set; }

    public int Order { get; private set; }

    public AssessmentRoundType Type { get; private set; }

    public AssessmentDifficulty Difficulty { get; private set; }

    public int DurationInMinutes { get; private set; }

    public int TargetQuestionCount { get; private set; }

    public IReadOnlyCollection<AssessmentRoundCompetency> Competencies =>
        _competencies.AsReadOnly();

    public static AssessmentRound Create(
        Guid assessmentStrategyId,
        int order,
        AssessmentRoundType type,
        AssessmentDifficulty difficulty,
        int durationInMinutes,
        int targetQuestionCount,
        IReadOnlyCollection<AssessmentRoundCompetency> competencies)
    {
        return new AssessmentRound(
            assessmentStrategyId,
            order,
            type,
            difficulty,
            durationInMinutes,
            targetQuestionCount,
            competencies);
    }
}
