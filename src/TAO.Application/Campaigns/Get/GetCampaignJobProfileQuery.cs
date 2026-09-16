using MediatR;
using TAO.Application.JobProfiles.Get;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

public sealed record GetCampaignJobProfileQuery(
    Guid CampaignId)
    : IRequest<Result<JobProfileResponse>>;
