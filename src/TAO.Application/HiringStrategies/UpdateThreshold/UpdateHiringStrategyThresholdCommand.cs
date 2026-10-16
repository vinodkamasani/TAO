using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.UpdateThreshold;

public sealed record UpdateHiringStrategyThresholdCommand(
    Guid CampaignId,
    int ThresholdPercentage)
    : IRequest<Result>;