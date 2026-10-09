namespace TAO.Application.AssessmentResults.GetCompletedAssessmentCandidates;

public sealed record GetCompletedAssessmentCandidatesResponse(
    Guid CandidateApplicationId,
    string CandidateName,
    string Email,
    string Phone,
    string? LinkedInUrl,
    string? CurrentCompany,
    string? CurrentLocation,
    Guid AssessmentSessionId,
    DateTime CompletedOn);