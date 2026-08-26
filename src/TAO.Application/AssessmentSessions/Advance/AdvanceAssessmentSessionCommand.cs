using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Advance;

public sealed record AdvanceAssessmentSessionCommand(
    Guid AssessmentSessionId)
    : IRequest<Result>;