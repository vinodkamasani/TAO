

namespace TAO.Application.AssessmentSessions.Advance;

public sealed record AdvanceAssessmentSessionResponse(
 Guid? QuestionId,
 int? QuestionOrder,
 string? PrimaryQuestion,
 IReadOnlyCollection<string> Competencies,
 Guid? RoundId,
 int? RoundOrder,
 string? RoundType,
 int? RoundDurationInMinutes,
 bool IsNewRound,
 bool AssessmentCompleted);
