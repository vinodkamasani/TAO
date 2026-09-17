using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Application.JobProfiles.Create;

public sealed record CreateJobProfileResponse(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    string OriginalJobDescription,
    MarkdownContent GeneratedContent,
    StructuredContent StructuredProfile,
    JobProfileStatus Status,
    DateTime GeneratedOn);