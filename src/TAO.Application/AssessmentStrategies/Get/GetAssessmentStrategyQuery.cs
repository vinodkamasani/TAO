using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Get;

public sealed record GetAssessmentStrategyQuery(
    Guid CampaignId)
    : IRequest<Result<GetAssessmentStrategyResponse>>;