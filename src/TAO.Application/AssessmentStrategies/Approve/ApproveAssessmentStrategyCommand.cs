using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Approve;

public sealed record ApproveAssessmentStrategyCommand(
    Guid AssessmentStrategyId,
    Guid ApprovedByUserId) : IRequest<Result>;