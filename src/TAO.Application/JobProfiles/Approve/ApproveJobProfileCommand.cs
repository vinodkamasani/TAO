using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Approve;

public sealed record ApproveJobProfileCommand(
Guid JobProfileId,
Guid ApprovedByUserId)
: IRequest<Result>;
