using System.Text.Json.Serialization;

namespace TAO.AI.Providers.Ollama.Contracts;

internal sealed class OllamaGenerateRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    [JsonPropertyName("stream")]
    public bool Stream { get; init; } = false;

    [JsonPropertyName("format")]
    public string Format { get; init; } = "json";
}