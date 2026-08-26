using System.Text.Json.Serialization;

namespace TAO.Domain.ValueObjects;

public sealed record AssessmentQuestionCompetencyEvaluation
{
    [JsonConstructor]
    private AssessmentQuestionCompetencyEvaluation(
        string name,
        byte score)
    {
        Name = ValidateName(name);
        ValidateScore(score);

        Score = score;
    }

    public string Name { get; }

    public byte Score { get; }

    public static AssessmentQuestionCompetencyEvaluation Create(
        string name,
        byte score)
    {
        return new AssessmentQuestionCompetencyEvaluation(
            name,
            score);
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Competency name is required.",
                nameof(name));
        }

        return name.Trim();
    }

    private static void ValidateScore(byte score)
    {
        if (score > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(score),
                "Competency score must be between 0 and 100.");
        }
    }
}