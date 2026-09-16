using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.JobProfiles.Regenerate;

internal sealed class RegenerateJobProfileCommandHandler
    : IRequestHandler<RegenerateJobProfileCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IJobProfileGenerator _jobProfileGenerator;

    public RegenerateJobProfileCommandHandler(
        IApplicationDbContext context,
        IJobProfileGenerator jobProfileGenerator)
    {
        _context = context;
        _jobProfileGenerator = jobProfileGenerator;
    }

    public async Task<Result> Handle(
        RegenerateJobProfileCommand request,
        CancellationToken cancellationToken)
    {
        var jobProfile = await _context
            .Set<JobProfile>()
            .FirstOrDefaultAsync(
                jp => jp.Id == request.JobProfileId,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"Job Profile '{request.JobProfileId}' was not found."));
        }

        var aiResult = await _jobProfileGenerator.GenerateAsync(
            request.OriginalJobDescription,
            cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result.Failure(aiResult.Error);
        }

        jobProfile.UpdateGeneratedContent(
            request.OriginalJobDescription,
            aiResult.Value.GeneratedContent,
            aiResult.Value.StructuredProfile,
            aiResult.Value.Prompt,
            aiResult.Value.RawResponse,
            aiResult.Value.ProviderName,
            aiResult.Value.ModelName,
            aiResult.Value.PromptVersion);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}