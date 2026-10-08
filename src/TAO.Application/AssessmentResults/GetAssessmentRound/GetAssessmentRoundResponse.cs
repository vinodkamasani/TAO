namespace TAO.Application.AssessmentResults.GetAssessmentRound;

public sealed record GetAssessmentRoundResponse(
    Guid AssessmentSessionId,
    Guid RoundId,
    int Order,
    string RoundType,
    byte Score,
    byte Confidence,
    IReadOnlyCollection<string> Strengths,
    IReadOnlyCollection<string> Gaps,
    IReadOnlyCollection<string> Evidence,
    IReadOnlyCollection<AssessmentQuestionSummary> Questions);

public sealed record AssessmentQuestionSummary(
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