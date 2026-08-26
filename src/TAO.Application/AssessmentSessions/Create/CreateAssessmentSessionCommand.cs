using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Create;

public sealed record CreateAssessmentSessionCommand(
    Guid CandidateApplicationId,
    Guid AssessmentStrategyId)
    : IRequest<Result<Guid>>;