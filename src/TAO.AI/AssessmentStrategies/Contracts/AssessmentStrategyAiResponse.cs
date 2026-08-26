namespace TAO.AI.AssessmentStrategies.Contracts;

public sealed class AssessmentStrategyAiResponse
{
    public required string AssessmentName { get; init; }

    public required IReadOnlyCollection<AssessmentRoundAiResponse> Rounds { get; init; }
}