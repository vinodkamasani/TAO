namespace TAO.AI.AssessmentStrategies.Contracts;

public sealed class AssessmentCompetencyAiResponse
{
    public required string Name { get; init; }

    public required string Priority { get; init; }
    public required byte MinimumPassPercentage { get; init; }
}