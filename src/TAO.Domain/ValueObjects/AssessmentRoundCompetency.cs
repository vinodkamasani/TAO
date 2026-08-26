namespace TAO.Domain.ValueObjects;

public sealed record AssessmentRoundCompetency
{
    public required string Name { get; init; }

    public required string Priority { get; init; }
    public required byte MinimumPassPercentage { get; init; }
}