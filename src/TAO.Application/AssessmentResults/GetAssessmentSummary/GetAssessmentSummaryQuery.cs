using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentSummary;

public sealed record GetAssessmentSummaryQuery(
    Guid AssessmentSessionId)
    : IRequest<Result<GetAssessmentSummaryResponse>>;