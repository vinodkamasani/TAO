using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.CandidateApplications.Get;

public sealed record GetCandidateApplicationsQuery(
    Guid CampaignId)
    : IRequest<Result<IReadOnlyCollection<GetCandidateApplicationsResponse>>>;