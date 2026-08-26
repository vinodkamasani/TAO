namespace TAO.Api.Endpoints.AssessmentSessions.Create;

public sealed record CreateAssessmentSessionRequest(
    Guid CandidateApplicationId,
    Guid AssessmentStrategyId);