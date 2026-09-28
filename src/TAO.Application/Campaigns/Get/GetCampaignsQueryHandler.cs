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
    private readonly ICurrentUser _currentUser;

    public GetCampaignsQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<IEnumerable<CampaignResponse>>> Handle(
        GetCampaignsQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<IEnumerable<CampaignResponse>>.Failure(
                Error.Unauthorized(
                    "Campaign.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<IEnumerable<CampaignResponse>>.Failure(
                Error.Unauthorized(
                    "Campaign.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        var campaigns = await _context
            .Set<Campaign>()
            .AsNoTracking()
            .Where(c => c.OrganizationId == organizationId.Value)
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
