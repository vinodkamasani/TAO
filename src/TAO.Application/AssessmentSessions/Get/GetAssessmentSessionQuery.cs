using MediatR;
using TAO.Application.AssessmentSessions.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Get;

public sealed record GetAssessmentSessionQuery(
    Guid AssessmentSessionId)
    : IRequest<Result<AssessmentSessionResponse>>;