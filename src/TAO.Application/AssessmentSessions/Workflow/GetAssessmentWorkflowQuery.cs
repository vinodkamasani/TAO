using MediatR;
using TAO.Application.AssessmentSessions.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Workflow;

public sealed record GetAssessmentWorkflowQuery(
    Guid AssessmentSessionId)
    : IRequest<Result<AssessmentWorkflowResponse>>;