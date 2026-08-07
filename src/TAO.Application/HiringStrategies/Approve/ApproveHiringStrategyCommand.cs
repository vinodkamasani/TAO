
using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Approve;

public sealed record ApproveHiringStrategyCommand(
    Guid HiringStrategyId,
    Guid ApprovedByUserId)
    : IRequest<Result>;
