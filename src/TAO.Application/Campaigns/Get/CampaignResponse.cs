namespace TAO.Application.Campaigns.Get;

public sealed record CampaignResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string ReferenceNumber,
    string RecruiterName,
    string HiringManagerName,
    int NumberOfOpenings,
    string Status,
    DateTime CreatedOn,
    DateTime? ModifiedOn);
