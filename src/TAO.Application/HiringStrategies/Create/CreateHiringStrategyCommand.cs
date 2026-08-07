using MediatR;
using TAO.SharedKernel.Results;

public sealed record CreateHiringStrategyCommand(
    Guid CampaignId)
    : IRequest<Result<Guid>>;