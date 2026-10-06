namespace TAO.Application.Candidates.GetAssessmentContext;

public sealed record GetCandidateAssessmentContextResponse(
    Guid CandidateApplicationId,
    Guid AssessmentStrategyId);