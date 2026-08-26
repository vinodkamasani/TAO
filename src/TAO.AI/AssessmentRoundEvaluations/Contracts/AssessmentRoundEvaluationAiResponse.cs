namespace TAO.AI.AssessmentRoundEvaluations.Contracts;

public sealed class AssessmentRoundEvaluationAiResponse
{
    public byte Confidence { get; init; }

    public IReadOnlyCollection<string> Strengths { get; init; } = [];

    public IReadOnlyCollection<string> Gaps { get; init; } = [];

    public IReadOnlyCollection<string> Evidence { get; init; } = [];
}