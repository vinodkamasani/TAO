using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Generate;
using TAO.Application.Common.Interfaces;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Get;

internal sealed class GetJobProfileQueryHandler
    : IRequestHandler<GetJobProfileQuery, Result<JobProfileResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetJobProfileQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<JobProfileResponse>> Handle(
        GetJobProfileQuery request,
        CancellationToken cancellationToken)
    {

        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<JobProfileResponse>.Failure(
                Error.Unauthorized(
                    "JobProfileResponse.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<JobProfileResponse>.Failure(
                Error.Unauthorized(
                    "JobProfileResponse.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        var jobProfile = await _context
            .Set<Domain.Entities.JobProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                jp => jp.Id == request.JobProfileId && jp.OrganizationId == organizationId.Value,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result<JobProfileResponse>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"Job Profile '{request.JobProfileId}' was not found."));
        }

        var response = new JobProfileResponse(
            jobProfile.Id,
            jobProfile.CampaignId,
            jobProfile.OriginalJobDescription,
            jobProfile.GeneratedContent.Value,
            jobProfile.StructuredProfile.Value,
            jobProfile.Status,
            jobProfile.GeneratedOn);

        return Result<JobProfileResponse>.Success(response);
    }
}