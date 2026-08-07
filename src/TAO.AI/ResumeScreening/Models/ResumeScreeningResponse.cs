using System.Text.Json;

namespace TAO.AI.ResumeScreening.Models;

internal sealed class ResumeScreeningResponse
{
    public required byte OverallMatchPercentage { get; init; }

    public required bool IsRecommended { get; init; }

    public string GeneratedMarkdown { get; init; }

    public required JsonElement StructuredContent { get; init; }
}