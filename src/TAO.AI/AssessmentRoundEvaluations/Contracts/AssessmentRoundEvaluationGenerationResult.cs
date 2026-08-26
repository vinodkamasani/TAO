namespace TAO.AI.AssessmentRoundEvaluations.Contracts;

public sealed class AssessmentRoundEvaluationGenerationResult
{
    public string Prompt { get; init; } = null!;

    public string RawResponse { get; init; } = null!;

    public string ProviderName { get; init; } = null!;

    public string ModelName { get; init; } = null!;

    public int PromptVersion { get; init; }

    public byte Confidence { get; init; }

    public IReadOnlyCollection<string> Strengths { get; init; } = [];

    public IReadOnlyCollection<string> Gaps { get; init; } = [];

    public IReadOnlyCollection<string> Evidence { get; init; } = [];
}