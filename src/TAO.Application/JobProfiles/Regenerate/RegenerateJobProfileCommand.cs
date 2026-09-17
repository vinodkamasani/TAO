using MediatR;
using TAO.Application.JobProfiles.Common;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Regenerate;

public sealed record RegenerateJobProfileCommand(
    Guid JobProfileId,
    string OriginalJobDescription)
    : IRequest<Result<JobProfileResponse>>;