using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

public sealed record GetCampaignsQuery
    : IRequest<Result<IEnumerable<CampaignResponse>>>;
