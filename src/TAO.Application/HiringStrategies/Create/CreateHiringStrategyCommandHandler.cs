using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Create;

internal sealed class CreateHiringStrategyCommandHandler
    : IRequestHandler<CreateHiringStrategyCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHiringStrategyGenerator _hiringStrategyGenerator;

    public CreateHiringStrategyCommandHandler(
        IApplicationDbContext context,
        IHiringStrategyGenerator hiringStrategyGenerator)
    {
        _context = context;
        _hiringStrategyGenerator = hiringStrategyGenerator;
    }

    public async Task<Result<Guid>> Handle(
        CreateHiringStrategyCommand request,
        CancellationToken cancellationToken)
    {
        var campaign = await _context
            .Set<Campaign>()
            .FirstOrDefaultAsync(
                c => c.Id == request.CampaignId,
                cancellationToken);

        if (campaign is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign '{request.CampaignId}' was not found."));
        }

        var jobProfile = await _context
            .Set<JobProfile>()
            .FirstOrDefaultAsync(
                jp => jp.CampaignId == request.CampaignId,
                cancellationToken);


        if (jobProfile is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"No Job Profile found for Campaign '{request.CampaignId}'."));
        }


        if (jobProfile.Status != JobProfileStatus.Approved)
        {
            return Result<Guid>.Failure(
                Error.Validation(
                    "JobProfile.NotApproved",
                    "The Job Profile must be approved before generating a Hiring Strategy."));
        }

        var aiResult = await _hiringStrategyGenerator.GenerateAsync(
            jobProfile,
            cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result<Guid>.Failure(aiResult.Error);
        }

        var existingHiringStrategy = await _context
                        .Set<HiringStrategy>()
                        .AnyAsync(
                            hs => hs.CampaignId == request.CampaignId,
                            cancellationToken);

                            if (existingHiringStrategy)
                            {
                                return Result<Guid>.Failure(
                                    Error.Conflict(
                                        "HiringStrategy.AlreadyExists",
                                        $"A Hiring Strategy already exists for Campaign '{request.CampaignId}'."));
                            }

        var hiringStrategy = HiringStrategy.Create(
            campaign.OrganizationId,
            campaign.Id,
            aiResult.Value.Prompt,
            aiResult.Value.RawResponse,
            aiResult.Value.ProviderName,
            aiResult.Value.ModelName,
            aiResult.Value.PromptVersion,
            aiResult.Value.Content,
            aiResult.Value.StructuredContent);

        _context
            .Set<HiringStrategy>()
            .Add(hiringStrategy);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(hiringStrategy.Id);
    }
}