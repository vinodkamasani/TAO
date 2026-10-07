namespace TAO.Application.AssessmentQuestions.Skip;

public sealed record SkipAssessmentQuestionResponse(
    Guid AssessmentQuestionId,
    bool IsSkipped);