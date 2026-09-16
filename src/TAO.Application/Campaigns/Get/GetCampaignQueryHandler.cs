using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

internal sealed class GetCampaignQueryHandler
    : IRequestHandler<GetCampaignQuery, Result<CampaignResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCampaignQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CampaignResponse>> Handle(
        GetCampaignQuery request,
        CancellationToken cancellationToken)
    {
        var campaign = await _context
            .Set<Campaign>()
            .AsNoTracking()
            .Join(
                _context.Set<User>(),
                campaign => campaign.RecruiterId,
                recruiter => recruiter.Id,
                (campaign, recruiter) => new { campaign, recruiter })
            .Join(
                _context.Set<User>(),
                cr => cr.campaign.HiringManagerId,
                hiringManager => hiringManager.Id,
                (cr, hiringManager) => new { cr.campaign, cr.recruiter, hiringManager })
            .Where(x => x.campaign.Id == request.CampaignId)
            .Select(x => new CampaignResponse(
                x.campaign.Id,
                x.campaign.OrganizationId,
                x.campaign.Name,
                x.campaign.ReferenceNumber,
                $"{x.recruiter.FirstName} {x.recruiter.LastName}",
                $"{x.hiringManager.FirstName} {x.hiringManager.LastName}",
                x.campaign.NumberOfOpenings,
                x.campaign.Status.ToString(),
                x.campaign.CreatedOn,
                x.campaign.ModifiedOn))
            .FirstOrDefaultAsync(cancellationToken);

        if (campaign is null)
        {
            return Result<CampaignResponse>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign with ID '{request.CampaignId}' was not found."));
        }

        return Result<CampaignResponse>.Success(campaign);
    }
}
