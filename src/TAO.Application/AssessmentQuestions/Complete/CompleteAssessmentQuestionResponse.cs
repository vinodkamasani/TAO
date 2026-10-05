
namespace TAO.Application.AssessmentQuestions.Complete
{
    public sealed record CompleteAssessmentQuestionResponse(
    Guid AssessmentQuestionId,
    bool IsEvaluated);
}
