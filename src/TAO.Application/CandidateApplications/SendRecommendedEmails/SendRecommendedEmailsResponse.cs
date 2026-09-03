namespace TAO.Application.CandidateApplications.SendRecommendedEmails;

public sealed record SendRecommendedEmailsResponse(
    Guid CampaignId,
    int TotalRecommendedCandidates,
    int EmailsSent);