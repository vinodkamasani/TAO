namespace TAO.Application.AssessmentResults.GetAssessmentQuestionCode;

public sealed record GetAssessmentQuestionCodeResponse(
    Guid AssessmentSessionId,
    Guid RoundId,
    Guid QuestionId,
    string Code);