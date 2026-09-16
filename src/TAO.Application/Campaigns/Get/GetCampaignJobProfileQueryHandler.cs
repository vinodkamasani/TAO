using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Application.JobProfiles.Get;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

internal sealed class GetCampaignJobProfileQueryHandler
    : IRequestHandler<GetCampaignJobProfileQuery, Result<JobProfileResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCampaignJobProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<JobProfileResponse>> Handle(
        GetCampaignJobProfileQuery request,
        CancellationToken cancellationToken)
    {
        var jobProfile = await _context
            .Set<JobProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                jp => jp.CampaignId == request.CampaignId,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result<JobProfileResponse>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"Job Profile for campaign '{request.CampaignId}' was not found."));
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
