using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

internal sealed class GetCampaignWorkflowStateQueryHandler
    : IRequestHandler<GetCampaignWorkflowStateQuery, Result<CampaignWorkflowStateResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCampaignWorkflowStateQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CampaignWorkflowStateResponse>> Handle(
        GetCampaignWorkflowStateQuery request,
        CancellationToken cancellationToken)
    {
        var campaign = await _context
            .Set<Campaign>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CampaignId, cancellationToken);

        if (campaign is null)
        {
            return Result<CampaignWorkflowStateResponse>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign with ID '{request.CampaignId}' was not found."));
        }

        // Get related entities
        var jobProfile = await _context
            .Set<JobProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(jp => jp.CampaignId == request.CampaignId, cancellationToken);

        var hiringStrategy = await _context
            .Set<HiringStrategy>()
            .AsNoTracking()
            .FirstOrDefaultAsync(hs => hs.CampaignId == request.CampaignId, cancellationToken);

        var assessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .AsNoTracking()
            .FirstOrDefaultAsync(asst => asst.CampaignId == request.CampaignId, cancellationToken);

        var resumeImport = await _context
            .Set<ResumeImport>()
            .AsNoTracking()
            .FirstOrDefaultAsync(ri => ri.CampaignId == request.CampaignId, cancellationToken);

        // Determine workflow stage and completion
        var (currentStage, completionPercentage) = DetermineWorkflowStage(
            jobProfile,
            hiringStrategy,
            assessmentStrategy,
            resumeImport);

        var response = new CampaignWorkflowStateResponse(
            campaign.Id,
            campaign.Name,
            currentStage,
            completionPercentage,

            // Job Profile
            jobProfile is not null,
            jobProfile?.Status.ToString(),
            jobProfile?.CreatedOn,
            jobProfile?.ApprovedOn,

            // Hiring Strategy
            hiringStrategy is not null,
            hiringStrategy?.Status.ToString(),
            hiringStrategy?.CreatedOn,
            hiringStrategy?.ApprovedOn,

            // Assessment Strategy
            assessmentStrategy is not null,
            assessmentStrategy?.Status.ToString(),
            assessmentStrategy?.CreatedOn,
            assessmentStrategy?.ApprovedOn,

            // Resume Import
            resumeImport is not null,
            resumeImport?.Status.ToString(),
            resumeImport?.TotalFiles ?? 0,
            resumeImport?.SuccessfulFiles ?? 0,
            resumeImport?.FailedFiles ?? 0,
            resumeImport?.ModifiedOn);

        return Result<CampaignWorkflowStateResponse>.Success(response);
    }

    private static (string Stage, int CompletionPercentage) DetermineWorkflowStage(
        JobProfile? jobProfile,
        HiringStrategy? hiringStrategy,
        AssessmentStrategy? assessmentStrategy,
        ResumeImport? resumeImport)
    {
        const int totalSteps = 4;
        int completedSteps = 0;
        string currentStage;

        // Stage 1: Job Profile
        if (jobProfile is null)
        {
            currentStage = "New - Job Profile Pending";
        }
        else if (jobProfile.Status.ToString() == "Generated")
        {
            currentStage = "Job Profile Generated - Awaiting Approval";
        }
        else // Approved
        {
            completedSteps++;

            // Stage 2: Hiring Strategy
            if (hiringStrategy is null)
            {
                currentStage = "Job Profile Approved - Hiring Strategy Pending";
            }
            else if (hiringStrategy.Status.ToString() == "Generated")
            {
                currentStage = "Hiring Strategy Generated - Awaiting Approval";
            }
            else // Approved
            {
                completedSteps++;

                // Stage 3: Resume Import
                if (resumeImport is null)
                {
                    currentStage = "Hiring Strategy Approved - Resumes Pending";
                }
                else if (resumeImport.Status.ToString() == "Queued" || 
                         resumeImport.Status.ToString() == "Processing")
                {
                    currentStage = $"Resumes Importing ({resumeImport.SuccessfulFiles}/{resumeImport.TotalFiles})";
                }
                else if (resumeImport.Status.ToString() == "Failed")
                {
                    currentStage = "Resume Import Failed";
                }
                else // Completed
                {
                    completedSteps++;

                    // Stage 4: Assessment Strategy
                    if (assessmentStrategy is null)
                    {
                        currentStage = "Resumes Imported - Assessment Strategy Pending";
                    }
                    else if (assessmentStrategy.Status.ToString() == "Generated")
                    {
                        currentStage = "Assessment Strategy Generated - Awaiting Approval";
                    }
                    else // Approved
                    {
                        completedSteps++;
                        currentStage = "Ready for Assessment";
                    }
                }
            }
        }

        int completionPercentage = (completedSteps * 100) / totalSteps;

        return (currentStage, completionPercentage);
    }
}
