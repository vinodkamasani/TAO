namespace TAO.AI.ResumeScreening.Contracts;

public sealed class ResumeScreeningRequest
{
    public required string JobProfile { get; init; }

    public required string HiringStrategy { get; init; }

    public required string ResumeProfile { get; init; }
}