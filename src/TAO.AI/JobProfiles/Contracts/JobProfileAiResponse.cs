using System.Text.Json;

namespace TAO.AI.JobProfiles.Contracts;

public sealed record JobProfileAiResponse
{
    public required string GeneratedMarkdown { get; init; }

    public required JsonElement StructuredProfile { get; init; }
}