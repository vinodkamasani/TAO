using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

public sealed record GetCampaignQuery(
    Guid CampaignId)
    : IRequest<Result<CampaignResponse>>;
