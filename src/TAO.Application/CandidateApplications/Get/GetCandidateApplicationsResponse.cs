namespace TAO.Application.CandidateApplications.Get;

public sealed record GetCandidateApplicationsResponse(
    Guid Id,
    string CandidateName,
    string Email,
    string Phone,
    string? LinkedInUrl,
    string? CurrentCompany,
    string? CurrentLocation,
    byte OverallMatchPercentage,
    string Status,
    DateTime ResumeUploadedOn,
    DateTime? LastScreenedOn);