namespace TAO.Application.AssessmentResults.GetAssessmentQuestionConversation;

public sealed record GetAssessmentQuestionConversationResponse(
    Guid AssessmentSessionId,
    Guid RoundId,
    Guid QuestionId,
    IReadOnlyCollection<AssessmentConversationMessage> Messages);

public sealed record AssessmentConversationMessage(
    string Role,
    string Content);