namespace TAO.AI.AssessmentQuestionEvaluations.Contracts;

public sealed class AssessmentQuestionEvaluationAiResponse
{
    public byte Score { get; set; }

    public byte Confidence { get; set; }

    public List<string> Strengths { get; set; } = [];

    public List<string> Gaps { get; set; } = [];

    public List<string> Evidence { get; set; } = [];

    public List<AssessmentQuestionCompetencyEvaluationAiResponse>
        Competencies
    { get; set; } = [];
}