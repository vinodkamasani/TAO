using TAO.Domain.Common;

namespace TAO.Domain.Entities;

public sealed class AssessmentCompetencyEvaluation : Entity
{
    private AssessmentCompetencyEvaluation()
    {
    }

    private AssessmentCompetencyEvaluation(
        Guid assessmentResultId,
        string competencyName,
        string priority,
        byte score,
        byte minimumPassPercentage)
    {
        AssessmentResultId = Guard.AgainstEmpty(
            assessmentResultId,
            nameof(AssessmentResultId));

        CompetencyName = Guard.AgainstNullOrWhiteSpace(
            competencyName,
            nameof(CompetencyName));

        Priority = Guard.AgainstNullOrWhiteSpace(
            priority,
            nameof(Priority));

        ValidatePercentage(
            score,
            nameof(score));

        ValidatePercentage(
            minimumPassPercentage,
            nameof(minimumPassPercentage));

        Score = score;
        MinimumPassPercentage = minimumPassPercentage;
    }

    public Guid AssessmentResultId { get; private set; }

    public string CompetencyName { get; private set; } = null!;

    public string Priority { get; private set; } = null!;

    public byte Score { get; private set; }

    public byte MinimumPassPercentage { get; private set; }

    public bool IsPassed =>
        Score >= MinimumPassPercentage;

    public static AssessmentCompetencyEvaluation Create(
        Guid assessmentResultId,
        string competencyName,
        string priority,
        byte score,
        byte minimumPassPercentage)
    {
        return new AssessmentCompetencyEvaluation(
            assessmentResultId,
            competencyName,
            priority,
            score,
            minimumPassPercentage);
    }

    public void Update(
        byte score,
        byte minimumPassPercentage)
    {
        ValidatePercentage(
            score,
            nameof(score));

        ValidatePercentage(
            minimumPassPercentage,
            nameof(minimumPassPercentage));

        Score = score;
        MinimumPassPercentage = minimumPassPercentage;
    }

    private static void ValidatePercentage(
        byte value,
        string parameterName)
    {
        if (value > 100)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Percentage must be between 0 and 100.");
        }
    }
}