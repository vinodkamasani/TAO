using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentEvaluations.Evaluate;

public sealed record EvaluateAssessmentCommand(
    Guid AssessmentSessionId) : IRequest<Result>;