using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Get
{
    public sealed record GetHiringStrategyQuery(
      Guid CampaignId)
      : IRequest<Result<GetHiringStrategyResponse>>;
}
