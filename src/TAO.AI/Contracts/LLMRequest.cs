namespace TAO.AI.Contracts;

public sealed record LLMRequest
{
    public required string Prompt { get; init; }
    public string ResponseFormat { get; init; } = "json";
}