namespace TAO.Application.AssessmentQuestions.FollowUp;

public sealed record GenerateFollowUpResponse(
    Guid QuestionId,
    int Order,
    string Question,
    IReadOnlyCollection<string> Competencies,
    string RoundType,
    int RoundDurationInMinutes,
    bool IsFollowUpQuestion);