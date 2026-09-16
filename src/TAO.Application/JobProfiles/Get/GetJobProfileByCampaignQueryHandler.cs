using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Get;

internal sealed class GetJobProfileByCampaignQueryHandler
    : IRequestHandler<GetJobProfileByCampaignQuery, Result<JobProfileResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetJobProfileByCampaignQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<JobProfileResponse>> Handle(
        GetJobProfileByCampaignQuery request,
        CancellationToken cancellationToken)
    {
        var jobProfile = await _context
            .Set<Domain.Entities.JobProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                jp => jp.CampaignId == request.CampaignId,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result<JobProfileResponse>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"Job Profile for Campaign '{request.CampaignId}' was not found."));
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