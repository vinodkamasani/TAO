using System.Text.Json;

namespace TAO.AI.HiringStrategies.Contracts;

public sealed record HiringStrategyAiResponse
{
    public required string GeneratedMarkdown { get; init; }

    public required JsonElement StructuredContent { get; init; }
}