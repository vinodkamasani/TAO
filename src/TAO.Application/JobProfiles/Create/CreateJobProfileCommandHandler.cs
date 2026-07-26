using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Create;

internal sealed class CreateJobProfileCommandHandler
    : IRequestHandler<CreateJobProfileCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJobProfileGenerator _jobProfileGenerator;

    public CreateJobProfileCommandHandler(
        IApplicationDbContext context,
        IJobProfileGenerator jobProfileGenerator)
    {
        _context = context;
        _jobProfileGenerator = jobProfileGenerator;
    }

    public async Task<Result<Guid>> Handle(
        CreateJobProfileCommand request,
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

        var aiResult = await _jobProfileGenerator.GenerateAsync(
            request.OriginalJobDescription,
            cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result<Guid>.Failure(aiResult.Error);
        }

        var jobProfile = JobProfile.Create(
            campaign.OrganizationId,
            campaign.Id,
            request.OriginalJobDescription,
            aiResult.Value.Prompt,
            aiResult.Value.RawResponse,
            aiResult.Value.ProviderName,
            aiResult.Value.ModelName,
            aiResult.Value.PromptVersion,
            aiResult.Value.GeneratedContent,
            aiResult.Value.StructuredProfile);

        _context
            .Set<JobProfile>()
            .Add(jobProfile);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(jobProfile.Id);
    }
}