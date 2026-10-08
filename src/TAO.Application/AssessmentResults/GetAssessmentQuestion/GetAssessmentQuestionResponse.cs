namespace TAO.Application.AssessmentResults.GetAssessmentQuestion;

public sealed record GetAssessmentQuestionResponse(
    Guid AssessmentSessionId,
    Guid RoundId,
    Guid QuestionId,
    int Order,
    string PrimaryQuestion,
    string Status,
    byte? Score,
    byte? Confidence,
    IReadOnlyCollection<string> Strengths,
    IReadOnlyCollection<string> Gaps,
    IReadOnlyCollection<string> Evidence,
    IReadOnlyCollection<AssessmentQuestionCompetencySummary> Competencies,
    bool HasConversation,
    bool HasCandidateCode);

public sealed record AssessmentQuestionCompetencySummary(
    string Name,
    byte Score);