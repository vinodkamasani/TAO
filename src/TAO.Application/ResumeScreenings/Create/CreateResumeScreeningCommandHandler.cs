using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.AI.ResumeScreening.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Application.ResumeScreenings.Services;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeScreenings.Create;

internal sealed class CreateResumeScreeningCommandHandler
    : IRequestHandler<CreateResumeScreeningCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IResumeScreeningGenerator _resumeScreeningGenerator;
    private readonly IResumeScreeningMarkdownGenerator _markdownGenerator;

    public CreateResumeScreeningCommandHandler(
       IApplicationDbContext context,
       IResumeScreeningGenerator resumeScreeningGenerator,
       IResumeScreeningMarkdownGenerator markdownGenerator)
    {
        _context = context;
        _resumeScreeningGenerator = resumeScreeningGenerator;
        _markdownGenerator = markdownGenerator;
    }


    public async Task<Result<Guid>> Handle(
        CreateResumeScreeningCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // Load Candidate Application
        // ------------------------------------------------------------

        var application = await _context
            .Set<CandidateApplication>()
            .FirstOrDefaultAsync(
                x => x.Id == request.CandidateApplicationId,
                cancellationToken);

        if (application is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "CandidateApplication.NotFound",
                    $"Candidate Application '{request.CandidateApplicationId}' was not found."));
        }

        // ------------------------------------------------------------
        // Load Campaign
        // ------------------------------------------------------------

        var campaign = await _context
            .Set<Campaign>()
            .FirstOrDefaultAsync(
                x => x.Id == application.CampaignId,
                cancellationToken);

        if (campaign is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign '{application.CampaignId}' was not found."));
        }

        // ------------------------------------------------------------
        // Load Job Profile
        // ------------------------------------------------------------

        var jobProfile = await _context
            .Set<JobProfile>()
            .FirstOrDefaultAsync(
                x => x.CampaignId == campaign.Id,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"Job Profile '{campaign.Id}' was not found."));
        }

        // ------------------------------------------------------------
        // Load Hiring Strategy
        // ------------------------------------------------------------

        var hiringStrategy = await _context
            .Set<HiringStrategy>()
            .FirstOrDefaultAsync(
                x => x.CampaignId == campaign.Id,
                cancellationToken);

        if (hiringStrategy is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "HiringStrategy.NotFound",
                    $"Hiring Strategy for Campaign '{campaign.Id}' was not found."));
        }

        // ------------------------------------------------------------
        // Load Resume Profile
        // ------------------------------------------------------------

        var resumeProfile = await _context
            .Set<ResumeProfile>()
            .FirstOrDefaultAsync(
                x => x.ApplicationId == application.Id,
                cancellationToken);

        if (resumeProfile is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "ResumeProfile.NotFound",
                    $"Resume Profile for Application '{application.Id}' was not found."));
        }

        // ------------------------------------------------------------
        // Generate Resume Screening
        // ------------------------------------------------------------

        var screeningResult =
            await _resumeScreeningGenerator.GenerateAsync(
                new ResumeScreeningRequest
                {
                    JobProfile = jobProfile.GeneratedContent.Value,
                    HiringStrategy = hiringStrategy.Content.Value,
                    ResumeProfile = resumeProfile.StructuredContent.Value
                },
                cancellationToken);

        if (screeningResult.IsFailure)
        {
            return Result<Guid>.Failure(
                screeningResult.Error!);
        }
        var result = screeningResult.Value!;

        // ------------------------------------------------------------------
        // Generate Markdown
        // ------------------------------------------------------------------

        var markdownContent =
            _markdownGenerator.Generate(result);

        // ------------------------------------------------------------------
        // Update Candidate Application
        // ------------------------------------------------------------------

        application.UpdateScreeningResult(
            result.OverallMatchPercentage,
            result.IsRecommended,
            DateTime.UtcNow);

        // ------------------------------------------------------------------
        // Replace Existing Resume Screening
        // ------------------------------------------------------------------

        var existingScreening = await _context
            .Set<ResumeScreening>()
            .FirstOrDefaultAsync(
                x => x.ApplicationId == application.Id,
                cancellationToken);

        if (existingScreening is not null)
        {
            _context
                .Set<ResumeScreening>()
                .Remove(existingScreening);
        }

        var screening = ResumeScreening.Create(
            application.OrganizationId,
            application.Id,
            markdownContent,
            StructuredContent.Create(
                result.StructuredContent));

        _context
            .Set<ResumeScreening>()
            .Add(screening);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(screening.Id);
    }
}