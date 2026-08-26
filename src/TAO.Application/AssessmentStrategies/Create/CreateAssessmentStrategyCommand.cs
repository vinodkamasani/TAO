using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Create;

public sealed record CreateAssessmentStrategyCommand(
    Guid CampaignId) : IRequest<Result<Guid>>;