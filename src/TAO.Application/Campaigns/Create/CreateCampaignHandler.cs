using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Create;

public sealed class CreateCampaignHandler
    : IRequestHandler<CreateCampaignCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateCampaignHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authenticated user
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null ||
            _currentUser.OrganizationId is null)
        {
            return Result<Guid>.Failure(
                Error.Unauthorized(
                    "Campaign.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId = _currentUser.OrganizationId.Value;

        // ---------------------------------------------------------
        // 2. Validate recruiter
        // ---------------------------------------------------------

        var recruiterExists = await _context
            .Set<User>()
            .AnyAsync(
                user =>
                    user.Id == request.RecruiterId
                    && user.OrganizationId == organizationId
                    && user.Role == UserRole.Recruiter
                    && user.Status == UserStatus.Active,
                cancellationToken);

        if (!recruiterExists)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "Campaign.RecruiterNotFound",
                    "The specified recruiter does not exist in the current organization."));
        }

        // ---------------------------------------------------------
        // 3. Validate hiring manager
        // ---------------------------------------------------------

        var hiringManagerExists = await _context
            .Set<User>()
            .AnyAsync(
                user =>
                    user.Id == request.HiringManagerId
                    && user.OrganizationId == organizationId
                    && user.Role == UserRole.HiringManager
                    && user.Status == UserStatus.Active,
                cancellationToken);

        if (!hiringManagerExists)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "Campaign.HiringManagerNotFound",
                    "The specified hiring manager does not exist in the current organization."));
        }

        // ---------------------------------------------------------
        // 4. Create campaign
        //
        // IMPORTANT:
        // Use organizationId from ICurrentUser.
        // Do not use request.OrganizationId.
        // ---------------------------------------------------------

        var campaign = Campaign.Create(
            organizationId,
            request.Name,
            request.ReferenceNumber,
            request.RecruiterId,
            request.HiringManagerId,
            request.NumberOfOpenings);

        _context
            .Set<Campaign>()
            .Add(campaign);

        // ---------------------------------------------------------
        // 5. Persist
        // ---------------------------------------------------------

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            campaign.Id);
    }
}