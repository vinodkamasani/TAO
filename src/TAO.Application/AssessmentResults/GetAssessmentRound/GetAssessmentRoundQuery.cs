using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentRound;

public sealed record GetAssessmentRoundQuery(
    Guid AssessmentSessionId,
    Guid RoundId)
    : IRequest<Result<GetAssessmentRoundResponse>>;