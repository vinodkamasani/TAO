namespace TAO.AI.AssessmentQuestions.Contracts;

public sealed record AssessmentQuestionAiResponse
{
    public required string Question { get; init; }

    public required IReadOnlyCollection<string> Competencies { get; init; }
}