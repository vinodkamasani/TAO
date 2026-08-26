namespace TAO.AI.AssessmentFollowUps.Contracts;

public sealed class AssessmentFollowUpGenerationResult
{
    public string Prompt { get; init; } = null!;

    public string RawResponse { get; init; } = null!;

    public string ProviderName { get; init; } = null!;

    public string ModelName { get; init; } = null!;

    public int PromptVersion { get; init; }

    public AssessmentFollowUpAiResponse Response { get; init; } = null!;
}