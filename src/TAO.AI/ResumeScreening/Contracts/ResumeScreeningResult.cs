namespace TAO.AI.ResumeScreening.Contracts;

public sealed class ResumeScreeningResult
{
    public required string Prompt { get; init; }

    public required string RawResponse { get; init; }

    public required string ProviderName { get; init; }

    public required string ModelName { get; init; }

    public required int PromptVersion { get; init; }

    public required string MarkdownContent { get; init; }

    public required string StructuredContent { get; init; }

    public required byte OverallMatchPercentage { get; init; }

    public required bool IsRecommended { get; init; }
}