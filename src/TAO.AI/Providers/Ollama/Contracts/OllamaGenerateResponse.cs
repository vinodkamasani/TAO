using System.Text.Json.Serialization;

namespace TAO.AI.Providers.Ollama.Contracts;

internal sealed class OllamaGenerateResponse
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("response")]
    public required string Response { get; init; }

    [JsonPropertyName("done")]
    public bool Done { get; init; }
}