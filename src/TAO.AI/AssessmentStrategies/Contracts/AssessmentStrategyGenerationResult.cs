using TAO.Domain.ValueObjects;

namespace TAO.AI.AssessmentStrategies.Contracts;

public sealed record AssessmentStrategyGenerationResult
{
    public required string Prompt { get; init; }

    public required string RawResponse { get; init; }

    public required string ProviderName { get; init; }

    public required string ModelName { get; init; }

    public required int PromptVersion { get; init; }

    public required StructuredContent StructuredContent { get; init; }
}