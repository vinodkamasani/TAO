using System.Text.Json;

namespace TAO.AI.ResumeParsing.Contracts;

internal sealed class ResumeParsingResponse
{
    public required string GeneratedJson { get; init; }

    public required JsonElement StructuredContent { get; init; }
}