using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Create;

public sealed class CreateCampaignHandler
    : IRequestHandler<CreateCampaignCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateCampaignHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        if (!await OrganizationExistsAsync(
                request.OrganizationId,
                cancellationToken))
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "Campaign.OrganizationNotFound",
                    "The specified organization does not exist."));
        }

        if (!await UserExistsAsync(
                request.RecruiterId,
                cancellationToken))
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "Campaign.RecruiterNotFound",
                    "The specified recruiter does not exist."));
        }

        if (!await UserExistsAsync(
                request.HiringManagerId,
                cancellationToken))
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "Campaign.HiringManagerNotFound",
                    "The specified hiring manager does not exist."));
        }

        var campaign = new Campaign(
            request.OrganizationId,
            request.Name,
            request.ReferenceNumber,
            request.RecruiterId,
            request.HiringManagerId,
            request.NumberOfOpenings);

        _context
            .Set<Campaign>()
            .Add(campaign);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(campaign.Id);
    }

    private Task<bool> OrganizationExistsAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        return _context
            .Set<Organization>()
            .AnyAsync(
                organization => organization.Id == organizationId,
                cancellationToken);
    }

    private Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _context
            .Set<User>()
            .AnyAsync(
                user => user.Id == userId,
                cancellationToken);
    }
}