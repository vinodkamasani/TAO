using TAO.Domain.Common;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class AssessmentQuestionEvaluation : Entity
{
    private readonly List<string> _strengths = [];
    private readonly List<string> _gaps = [];
    private readonly List<string> _evidence = [];
    private readonly List<AssessmentQuestionCompetencyEvaluation> _competencies = [];

    private AssessmentQuestionEvaluation()
    {
    }

    private AssessmentQuestionEvaluation(
        Guid assessmentQuestionId,
        byte score,
        byte confidence,
        IReadOnlyCollection<string> strengths,
        IReadOnlyCollection<string> gaps,
        IReadOnlyCollection<string> evidence,
        IReadOnlyCollection<AssessmentQuestionCompetencyEvaluation> competencies)
    {
        AssessmentQuestionId = Guard.AgainstEmpty(
            assessmentQuestionId,
            nameof(AssessmentQuestionId));

        ValidatePercentage(score, nameof(score));
        ValidatePercentage(confidence, nameof(confidence));

        ArgumentNullException.ThrowIfNull(strengths);
        ArgumentNullException.ThrowIfNull(gaps);
        ArgumentNullException.ThrowIfNull(evidence);
        ArgumentNullException.ThrowIfNull(competencies);

        Score = score;
        Confidence = confidence;

        AddStrings(_strengths, strengths);
        AddStrings(_gaps, gaps);
        AddStrings(_evidence, evidence);

        AddCompetencies(competencies);
    }

    public Guid AssessmentQuestionId { get; private set; }

    public byte Score { get; private set; }

    public byte Confidence { get; private set; }

    public IReadOnlyCollection<string> Strengths =>
        _strengths.AsReadOnly();

    public IReadOnlyCollection<string> Gaps =>
        _gaps.AsReadOnly();

    public IReadOnlyCollection<string> Evidence =>
        _evidence.AsReadOnly();

    public IReadOnlyCollection<AssessmentQuestionCompetencyEvaluation> Competencies =>
        _competencies.AsReadOnly();

    public static AssessmentQuestionEvaluation Create(
        Guid assessmentQuestionId,
        byte score,
        byte confidence,
        IReadOnlyCollection<string> strengths,
        IReadOnlyCollection<string> gaps,
        IReadOnlyCollection<string> evidence,
        IReadOnlyCollection<AssessmentQuestionCompetencyEvaluation> competencies)
    {
        return new AssessmentQuestionEvaluation(
            assessmentQuestionId,
            score,
            confidence,
            strengths,
            gaps,
            evidence,
            competencies);
    }

    public void Update(
        byte score,
        byte confidence,
        IReadOnlyCollection<string> strengths,
        IReadOnlyCollection<string> gaps,
        IReadOnlyCollection<string> evidence,
        IReadOnlyCollection<AssessmentQuestionCompetencyEvaluation> competencies)
    {
        ValidatePercentage(score, nameof(score));
        ValidatePercentage(confidence, nameof(confidence));

        ArgumentNullException.ThrowIfNull(strengths);
        ArgumentNullException.ThrowIfNull(gaps);
        ArgumentNullException.ThrowIfNull(evidence);
        ArgumentNullException.ThrowIfNull(competencies);

        Score = score;
        Confidence = confidence;

        _strengths.Clear();
        _gaps.Clear();
        _evidence.Clear();
        _competencies.Clear();

        AddStrings(_strengths, strengths);
        AddStrings(_gaps, gaps);
        AddStrings(_evidence, evidence);

        AddCompetencies(competencies);
    }

    private void AddCompetencies(
        IEnumerable<AssessmentQuestionCompetencyEvaluation> competencies)
    {
        foreach (var competency in competencies)
        {
            ArgumentNullException.ThrowIfNull(competency);

            _competencies.Add(competency);
        }
    }

    private static void AddStrings(
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