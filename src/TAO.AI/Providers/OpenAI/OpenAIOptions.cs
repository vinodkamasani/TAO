public sealed class OpenAIOptions
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; init; } = string.Empty;

    public string Model { get; init; } = "gpt-5-mini";

    public float Temperature { get; init; } = 0.1f;

    public int MaxOutputTokens { get; init; } = 4000;
}