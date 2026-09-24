using MediatR;
using TAO.Application.AssessmentStrategies.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Create;

public sealed record CreateAssessmentStrategyCommand(
    Guid CampaignId)
    : IRequest<Result<AssessmentStrategyResponse>>;