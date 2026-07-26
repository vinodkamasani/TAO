namespace TAO.Api.Endpoints.Campaigns.Create;

public sealed record CreateCampaignRequest(
    Guid OrganizationId,
    string Name,
    string ReferenceNumber,
    Guid RecruiterId,
    Guid HiringManagerId,
    int NumberOfOpenings);