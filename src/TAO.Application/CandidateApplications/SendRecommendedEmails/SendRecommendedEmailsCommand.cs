using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.CandidateApplications.SendRecommendedEmails;

public sealed record SendRecommendedEmailsCommand(
    Guid CampaignId)
    : IRequest<Result<SendRecommendedEmailsResponse>>;