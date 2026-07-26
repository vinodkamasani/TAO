namespace TAO.AI.Providers.Ollama;

public sealed class OllamaOptions
{
    public const string SectionName = "Ollama";

    public required string BaseUrl { get; init; }

    public required string Model { get; init; }
    public bool Stream { get; init; } = false;

    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(2);
}