namespace TAO.AI.Common;

public sealed class AIOptions
{
    public const string SectionName = "AI";

    public required string Provider { get; init; }
}