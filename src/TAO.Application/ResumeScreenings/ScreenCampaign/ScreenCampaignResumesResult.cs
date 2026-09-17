namespace TAO.Application.ResumeScreenings.ScreenCampaign;

public sealed record ScreenCampaignResumesResult(
    int TotalCandidates,
    int CandidatesAlreadyScreened,
    int CandidatesScreened,
    int RecommendedCandidates,
    int NotRecommendedCandidates,
    int FailedCandidates);