namespace TAO.AI.Contracts;

public sealed record LLMResponse
{
    public required string Content { get; init; }

    public required string ProviderName { get; init; }

    public required string ModelName { get; init; }
}