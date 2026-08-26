namespace TAO.AI.AssessmentQuestionEvaluations.Contracts;

public sealed class AssessmentQuestionCompetencyEvaluationAiResponse
{
    public string Name { get; set; } = null!;

    public byte Score { get; set; }
}