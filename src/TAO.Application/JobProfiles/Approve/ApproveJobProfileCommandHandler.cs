using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Generate;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Approve;

internal sealed class ApproveJobProfileCommandHandler
    : IRequestHandler<ApproveJobProfileCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ApproveJobProfileCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        ApproveJobProfileCommand request,
        CancellationToken cancellationToken)
    {

        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<Result>.Failure(
                Error.Unauthorized(
                    "ApproveJobProfile.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<Result>.Failure(
                Error.Unauthorized(
                    "ApproveJobProfile.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        var jobProfile = await _context
            .Set<JobProfile>()
            .FirstOrDefaultAsync(
                jp => jp.Id == request.JobProfileId && jp.OrganizationId == organizationId.Value,
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