using MediatR;
using TAO.Application.HiringStrategies.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Get
{
    public sealed record GetHiringStrategyQuery(
      Guid CampaignId)
      : IRequest<Result<HiringStrategyResponse>>;
}
