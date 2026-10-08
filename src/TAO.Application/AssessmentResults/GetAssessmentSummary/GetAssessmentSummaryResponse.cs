namespace TAO.Application.AssessmentResults.GetAssessmentSummary;

public sealed record GetAssessmentSummaryResponse(
    Guid AssessmentSessionId,
    Guid AssessmentResultId,
    byte OverallScore,
    byte OverallConfidence,
    string Recommendation,
    string ExecutiveSummary,
    IReadOnlyCollection<AssessmentCompetencySummary> Competencies,
    IReadOnlyCollection<AssessmentRoundSummary> Rounds);

public sealed record AssessmentCompetencySummary(
    string Name,
    string Priority,
    byte Score,
    byte MinimumPassPercentage,
    bool IsPassed);

public sealed record AssessmentRoundSummary(
    Guid RoundId,
    int Order,
    string RoundType,
    byte Score,
    byte Confidence,
    int TotalQuestions,
    int CompletedQuestions,
    int SkippedQuestions);