using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetCompletedAssessmentCandidates;

public sealed record GetCompletedAssessmentCandidatesQuery(
    Guid CampaignId)
    : IRequest<
        Result<
            IReadOnlyCollection<
                GetCompletedAssessmentCandidatesResponse>>>;