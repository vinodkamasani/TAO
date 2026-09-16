using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

public sealed record GetCampaignWorkflowStateQuery(
    Guid CampaignId)
    : IRequest<Result<CampaignWorkflowStateResponse>>;
