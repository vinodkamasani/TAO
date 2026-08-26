namespace TAO.Domain.ValueObjects;

public sealed record AssessmentCompetency
{
    public required string Name { get; init; }

    public required string Priority { get; init; }
}