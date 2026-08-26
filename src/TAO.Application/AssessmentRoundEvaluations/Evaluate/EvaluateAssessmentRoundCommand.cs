using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentRoundEvaluations.Evaluate;

public sealed record EvaluateAssessmentRoundCommand(
    Guid AssessmentSessionRoundId) : IRequest<Result>;