namespace TAO.AI.AssessmentStrategies.Contracts;

public sealed class AssessmentRoundAiResponse
{
    public required int Order { get; init; }

    public required string Type { get; init; }

    public required string Difficulty { get; init; }

    public required int DurationInMinutes { get; init; }

    public required int QuestionCount { get; init; }

    public required IReadOnlyCollection<AssessmentCompetencyAiResponse> Competencies { get; init; }
}