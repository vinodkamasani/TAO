using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Create;

public sealed record CreateJobProfileCommand(
 Guid CampaignId,
 string OriginalJobDescription)
 : IRequest<Result<Guid>>;
