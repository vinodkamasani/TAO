using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

internal sealed class GetCampaignsQueryHandler
    : IRequestHandler<GetCampaignsQuery, Result<IEnumerable<CampaignResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetCampaignsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<CampaignResponse>>> Handle(
        GetCampaignsQuery request,
        CancellationToken cancellationToken)
    {
        var campaigns = await _context
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
            .OrderByDescending(x => x.campaign.CreatedOn)
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
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<CampaignResponse>>.Success(campaigns);
    }
}
