using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeScreenings.ScreenCampaign;

public sealed record ScreenCampaignResumesCommand(
    Guid CampaignId)
    : IRequest<Result<ScreenCampaignResumesResult>>;