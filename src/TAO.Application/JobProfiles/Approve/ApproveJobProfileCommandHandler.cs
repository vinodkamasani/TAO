using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Approve;

internal sealed class ApproveJobProfileCommandHandler
    : IRequestHandler<ApproveJobProfileCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public ApproveJobProfileCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        ApproveJobProfileCommand request,
        CancellationToken cancellationToken)
    {
        var jobProfile = await _context
            .Set<JobProfile>()
            .FirstOrDefaultAsync(
                jp => jp.Id == request.JobProfileId,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"Job Profile '{request.JobProfileId}' was not found."));
        }

        jobProfile.Approve(request.ApprovedByUserId);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}