using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.AI.ResumeScreening.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Application.ResumeScreenings.Services;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeScreenings.ScreenCampaign;

internal sealed class ScreenCampaignResumesCommandHandler
    : IRequestHandler<
        ScreenCampaignResumesCommand,
        Result<ScreenCampaignResumesResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IResumeScreeningGenerator _resumeScreeningGenerator;
    private readonly IResumeScreeningMarkdownGenerator _markdownGenerator;

    public ScreenCampaignResumesCommandHandler(
        IApplicationDbContext context,
        IResumeScreeningGenerator resumeScreeningGenerator,
        IResumeScreeningMarkdownGenerator markdownGenerator)
    {
        _context = context;
        _resumeScreeningGenerator = resumeScreeningGenerator;
        _markdownGenerator = markdownGenerator;
    }

    public async Task<Result<ScreenCampaignResumesResult>> Handle(
        ScreenCampaignResumesCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // Load Campaign
        // ------------------------------------------------------------

        var campaign = await _context
            .Set<Campaign>()
            .FirstOrDefaultAsync(
                x => x.Id == request.CampaignId,
                cancellationToken);

        if (campaign is null)
        {
            return Result<ScreenCampaignResumesResult>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign '{request.CampaignId}' was not found."));
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
            return Result<ScreenCampaignResumesResult>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"Job Profile for Campaign '{campaign.Id}' was not found."));
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
            return Result<ScreenCampaignResumesResult>.Failure(
                Error.NotFound(
                    "HiringStrategy.NotFound",
                    $"Hiring Strategy for Campaign '{campaign.Id}' was not found."));
        }

        // ------------------------------------------------------------
        // Load Candidate Applications
        // ------------------------------------------------------------

        var applications = await _context
            .Set<CandidateApplication>()
            .Where(x => x.CampaignId == campaign.Id)
            .ToListAsync(cancellationToken);

        var totalCandidates = applications.Count;

        if (totalCandidates == 0)
        {
            return Result<ScreenCampaignResumesResult>.Success(
                new ScreenCampaignResumesResult(
                    TotalCandidates: 0,
                    CandidatesAlreadyScreened: 0,
                    CandidatesScreened: 0,
                    RecommendedCandidates: 0,
                    NotRecommendedCandidates: 0,
                    FailedCandidates: 0));
        }

        var applicationIds = applications
            .Select(x => x.Id)
            .ToList();

        // ------------------------------------------------------------
        // Load Resume Profiles
        // ------------------------------------------------------------

        var resumeProfiles = await _context
            .Set<ResumeProfile>()
            .Where(x => applicationIds.Contains(x.ApplicationId))
            .ToListAsync(cancellationToken);

        var resumeProfilesByApplicationId =
            resumeProfiles.ToDictionary(
                x => x.ApplicationId);

        // ------------------------------------------------------------
        // Load Existing Resume Screenings
        // ------------------------------------------------------------

        var screenedApplicationIds = await _context
            .Set<ResumeScreening>()
            .Where(x => applicationIds.Contains(x.ApplicationId))
            .Select(x => x.ApplicationId)
            .ToHashSetAsync(cancellationToken);

        // ------------------------------------------------------------
        // Determine Candidates That Need Screening
        // ------------------------------------------------------------

        var applicationsToScreen = applications
            .Where(x => !screenedApplicationIds.Contains(x.Id))
            .ToList();

        var candidatesAlreadyScreened =
            screenedApplicationIds.Count;

        var candidatesScreened = 0;
        var recommendedCandidates = 0;
        var notRecommendedCandidates = 0;
        var failedCandidates = 0;

        // ------------------------------------------------------------
        // Screen Candidates
        // ------------------------------------------------------------

        foreach (var application in applicationsToScreen)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!resumeProfilesByApplicationId.TryGetValue(
                    application.Id,
                    out var resumeProfile))
            {
                failedCandidates++;
                continue;
            }

            try
            {
                // ----------------------------------------------------
                // Generate Resume Screening
                // ----------------------------------------------------

                var screeningResult =
                    await _resumeScreeningGenerator.GenerateAsync(
                        new ResumeScreeningRequest
                        {
                            JobProfile =
                                jobProfile.GeneratedContent.Value,

                            HiringStrategy =
                                hiringStrategy.Content.Value,

                            ResumeProfile =
                                resumeProfile.StructuredContent.Value
                        },
                        cancellationToken);

                if (screeningResult.IsFailure)
                {
                    failedCandidates++;
                    continue;
                }

                var result = screeningResult.Value!;

                // ----------------------------------------------------
                // Generate Markdown
                // ----------------------------------------------------

                var markdownContent =
                    _markdownGenerator.Generate(result);

                // ----------------------------------------------------
                // Update Candidate Application
                // ----------------------------------------------------

                application.UpdateScreeningResult(
                    result.OverallMatchPercentage,
                    result.IsRecommended,
                    DateTime.UtcNow);

                // ----------------------------------------------------
                // Create Resume Screening
                // ----------------------------------------------------

                var screening = ResumeScreening.Create(
                    application.OrganizationId,
                    application.Id,
                    markdownContent,
                    StructuredContent.Create(
                        result.StructuredContent));

                _context
                    .Set<ResumeScreening>()
                    .Add(screening);

                candidatesScreened++;

                if (result.IsRecommended)
                {
                    recommendedCandidates++;
                }
                else
                {
                    notRecommendedCandidates++;
                }
            }
            catch
            {
                failedCandidates++;
            }
        }

        // ------------------------------------------------------------
        // Save Changes
        // ------------------------------------------------------------

        await _context.SaveChangesAsync(cancellationToken);

        // ------------------------------------------------------------
        // Return Summary
        // ------------------------------------------------------------

        return Result<ScreenCampaignResumesResult>.Success(
            new ScreenCampaignResumesResult(
                TotalCandidates: totalCandidates,
                CandidatesAlreadyScreened: candidatesAlreadyScreened,
                CandidatesScreened: candidatesScreened,
                RecommendedCandidates: recommendedCandidates,
                NotRecommendedCandidates: notRecommendedCandidates,
                FailedCandidates: failedCandidates));
    }
}