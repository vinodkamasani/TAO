using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Candidates.GetAssessmentContext;

public sealed record GetCandidateAssessmentContextQuery(
    Guid InvitationId)
    : IRequest<Result<GetCandidateAssessmentContextResponse>>;