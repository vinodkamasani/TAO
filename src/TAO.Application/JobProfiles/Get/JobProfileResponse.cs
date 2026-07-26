using TAO.Domain.Enums;

namespace TAO.Application.JobProfiles.Get;

public sealed record JobProfileResponse(
    Guid Id,
    Guid CampaignId,
    string OriginalJobDescription,
    string GeneratedContent,
    string StructuredProfile,
    JobProfileStatus Status,
    DateTime GeneratedOn);
