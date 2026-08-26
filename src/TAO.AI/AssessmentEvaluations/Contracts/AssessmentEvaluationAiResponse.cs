using TAO.Domain.Enums;

namespace TAO.AI.AssessmentEvaluations.Contracts;

public sealed class AssessmentEvaluationAiResponse
{
    public byte Confidence { get; init; }

    public string ExecutiveSummary { get; init; } = null!;

    public IReadOnlyCollection<string> Strengths { get; init; } = [];

    public IReadOnlyCollection<string> Gaps { get; init; } = [];

    public IReadOnlyCollection<string> Evidence { get; init; } = [];
}