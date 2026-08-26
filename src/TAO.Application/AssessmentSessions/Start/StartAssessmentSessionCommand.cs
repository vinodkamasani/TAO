using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Start;

public sealed record StartAssessmentSessionCommand(
    Guid AssessmentSessionId)
    : IRequest<Result>;