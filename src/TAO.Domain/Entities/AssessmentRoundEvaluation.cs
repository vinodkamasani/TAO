using TAO.Domain.Common;

namespace TAO.Domain.Entities;

public sealed class AssessmentRoundEvaluation : Entity
{
    private readonly List<string> _strengths = [];
    private readonly List<string> _gaps = [];
    private readonly List<string> _evidence = [];

    private AssessmentRoundEvaluation()
    {
    }

    private AssessmentRoundEvaluation(
        Guid assessmentSessionRoundId,
        byte score,
        byte confidence,
        IReadOnlyCollection<string> strengths,
        IReadOnlyCollection<string> gaps,
        IReadOnlyCollection<string> evidence)
    {
        AssessmentSessionRoundId  = Guard.AgainstEmpty(
            assessmentSessionRoundId,
            nameof(AssessmentSessionRoundId));

        ValidatePercentage(score, nameof(score));
        ValidatePercentage(confidence, nameof(confidence));

        ArgumentNullException.ThrowIfNull(strengths);
        ArgumentNullException.ThrowIfNull(gaps);
        ArgumentNullException.ThrowIfNull(evidence);

        Score = score;
        Confidence = confidence;

        AddItems(_strengths, strengths);
        AddItems(_gaps, gaps);
        AddItems(_evidence, evidence);
    }

    public Guid AssessmentSessionRoundId { get; private set; }

    public byte Score { get; private set; }

    public byte Confidence { get; private set; }

    public IReadOnlyCollection<string> Strengths =>
        _strengths.AsReadOnly();

    public IReadOnlyCollection<string> Gaps =>
        _gaps.AsReadOnly();

    public IReadOnlyCollection<string> Evidence =>
        _evidence.AsReadOnly();

    public static AssessmentRoundEvaluation Create(
        Guid assessmentSessionRoundId,
        byte score,
        byte confidence,
        IReadOnlyCollection<string> strengths,
        IReadOnlyCollection<string> gaps,
        IReadOnlyCollection<string> evidence)
    {
        return new AssessmentRoundEvaluation(
            assessmentSessionRoundId,
            score,
            confidence,
            strengths,
            gaps,
            evidence);
    }

    public void Update(
        byte score,
        byte confidence,
        IReadOnlyCollection<string> strengths,
        IReadOnlyCollection<string> gaps,
        IReadOnlyCollection<string> evidence)
    {
        ValidatePercentage(score, nameof(score));
        ValidatePercentage(confidence, nameof(confidence));

        ArgumentNullException.ThrowIfNull(strengths);
        ArgumentNullException.ThrowIfNull(gaps);
        ArgumentNullException.ThrowIfNull(evidence);

        Score = score;
        Confidence = confidence;

        _strengths.Clear();
        _gaps.Clear();
        _evidence.Clear();

        AddItems(_strengths, strengths);
        AddItems(_gaps, gaps);
        AddItems(_evidence, evidence);
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

    private static void AddItems(
        List<string> target,
        IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                target.Add(item.Trim());
            }
        }
    }
}