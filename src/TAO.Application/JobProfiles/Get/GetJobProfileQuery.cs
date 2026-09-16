using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Get;

public sealed record GetJobProfileQuery(
    Guid JobProfileId)
    : IRequest<Result<JobProfileResponse>>;
public sealed record GetJobProfileByCampaignQuery(
    Guid CampaignId)
    : IRequest<Result<JobProfileResponse>>;