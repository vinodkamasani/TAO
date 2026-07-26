using TAO.Domain.ValueObjects;

namespace TAO.AI.JobProfiles.Contracts;

public sealed record JobProfileGenerationResult
{
    public required string Prompt { get; init; }
    public required string RawResponse { get; init; }
    public required string ProviderName { get; init; }
    public required string ModelName { get; init; }
    public required int PromptVersion { get; init; }
    public required MarkdownContent GeneratedContent { get; init; }
    public required StructuredContent StructuredProfile { get; init; }
}
