using MediatR;
using TAO.Application.HiringStrategies.Create;
using TAO.SharedKernel.Results;

public sealed record CreateHiringStrategyCommand(
    Guid CampaignId)
  : IRequest<Result<CreateHiringStrategyResponse>>;