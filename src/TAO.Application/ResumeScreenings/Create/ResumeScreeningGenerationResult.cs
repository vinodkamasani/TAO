using TAO.Domain.ValueObjects;

namespace TAO.Application.ResumeScreenings.Create;

public sealed class ResumeScreeningGenerationResult
{
    public required byte OverallMatchPercentage { get; init; }

    public required bool IsRecommended { get; init; }

    public required MarkdownContent Content { get; init; }

    public required StructuredContent StructuredContent { get; init; }
}