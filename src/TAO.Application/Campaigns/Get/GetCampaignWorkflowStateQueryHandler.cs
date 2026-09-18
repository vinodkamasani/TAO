using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Campaigns.Get;

public sealed class GetCampaignWorkflowStateQueryHandler
    : IRequestHandler<
        GetCampaignWorkflowStateQuery,
        Result<CampaignWorkflowStateResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCampaignWorkflowStateQueryHandler(
        IApplicationDbContext context)
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
            .FirstOrDefaultAsync(
                x => x.Id == request.CampaignId,
                cancellationToken);

        if (campaign is null)
        {
            return Result<CampaignWorkflowStateResponse>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    "Campaign was not found."));
        }

        // ---------------------------------------------------------
        // 1. Job Profile
        // ---------------------------------------------------------

        var jobProfile = await _context
            .Set<JobProfile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CampaignId == request.CampaignId,
                cancellationToken);

        // ---------------------------------------------------------
        // 2. Hiring Strategy
        // ---------------------------------------------------------

        var hiringStrategy = await _context
            .Set<HiringStrategy>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CampaignId == request.CampaignId,
                cancellationToken);

        // ---------------------------------------------------------
        // 3. Resume Import
        // ---------------------------------------------------------

        var resumeImport = await _context
            .Set<ResumeImport>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CampaignId == request.CampaignId,
                cancellationToken);

        // ---------------------------------------------------------
        // 4. Candidate Screening
        // ---------------------------------------------------------

        var candidates = await _context
            .Set<CandidateApplication>()
            .AsNoTracking()
            .Where(x => x.CampaignId == request.CampaignId)
            .Select(x => new CandidateWorkflowData(
                x.Id,
                x.IsRecommended,
                x.LastScreenedOn))
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // 5. Assessment Strategy
        // ---------------------------------------------------------

        var assessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CampaignId == request.CampaignId,
                cancellationToken);

        // ---------------------------------------------------------
        // 6. Invitations
        // ---------------------------------------------------------

        var emailDeliveries = await _context
            .Set<EmailDelivery>()
            .AsNoTracking()
            .Where(x => x.CampaignId == request.CampaignId)
            .Select(x => new EmailDeliveryWorkflowData(
                x.CandidateApplicationId,
                x.Status,
                x.SentOn))
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // Determine Workflow
        // ---------------------------------------------------------

        var workflow = DetermineWorkflowStage(
            jobProfile,
            hiringStrategy,
            resumeImport,
            candidates,
            assessmentStrategy,
            emailDeliveries);

        var response = new CampaignWorkflowStateResponse(
            CampaignId: campaign.Id,
            CampaignName: campaign.Name,
            CurrentStage: workflow.CurrentStage,
            CompletionPercentage: workflow.CompletionPercentage,

            // Job Profile
            HasJobProfile: jobProfile is not null,
            JobProfileStatus: jobProfile?.Status.ToString(),
            JobProfileCreatedOn: jobProfile?.CreatedOn,
            JobProfileApprovedOn: jobProfile?.ApprovedOn,

            // Hiring Strategy
            HasHiringStrategy: hiringStrategy is not null,
            HiringStrategyStatus: hiringStrategy?.Status.ToString(),
            HiringStrategyCreatedOn: hiringStrategy?.CreatedOn,
            HiringStrategyApprovedOn: hiringStrategy?.ApprovedOn,

            // Assessment Strategy
            HasAssessmentStrategy: assessmentStrategy is not null,
            AssessmentStrategyStatus: assessmentStrategy?.Status.ToString(),
            AssessmentStrategyCreatedOn: assessmentStrategy?.CreatedOn,
            AssessmentStrategyApprovedOn: assessmentStrategy?.ApprovedOn,

            // Resume Import
            HasResumeImport: resumeImport is not null,
            ResumeImportStatus: resumeImport?.Status.ToString(),
            TotalResumes: resumeImport?.TotalFiles ?? 0,
            SuccessfulResumes: resumeImport?.SuccessfulFiles ?? 0,
            FailedResumes: resumeImport?.FailedFiles ?? 0,
            ResumeImportCompletedOn: null,

            // Candidate Screening
            HasCandidatesScreening:
                workflow.HasCandidatesScreening,

            CandidatesScreeningCompletedOn:
                workflow.CandidatesScreeningCompletedOn,

            // Invitations
            HasInvitations:
                workflow.HasInvitations,

            InvitationsCompletedOn:
                workflow.InvitationsCompletedOn);

        return Result<CampaignWorkflowStateResponse>.Success(response);
    }

    private static WorkflowStageResult DetermineWorkflowStage(
        JobProfile? jobProfile,
        HiringStrategy? hiringStrategy,
        ResumeImport? resumeImport,
        IReadOnlyCollection<CandidateWorkflowData> candidates,
        AssessmentStrategy? assessmentStrategy,
        IReadOnlyCollection<EmailDeliveryWorkflowData> emailDeliveries)
    {
        const int totalSteps = 6;

        var completedSteps = 0;

        // =========================================================
        // 1. JOB PROFILE
        // =========================================================

        if (jobProfile is null)
        {
            return CreateResult(
                "New - Job Profile Pending",
                completedSteps,
                totalSteps);
        }

        if (jobProfile.Status != JobProfileStatus.Approved)
        {
            return CreateResult(
                "Job Profile Generated - Awaiting Approval",
                completedSteps,
                totalSteps);
        }

        completedSteps++;

        // =========================================================
        // 2. HIRING STRATEGY
        // =========================================================

        if (hiringStrategy is null)
        {
            return CreateResult(
                "Job Profile Approved - Hiring Strategy Pending",
                completedSteps,
                totalSteps);
        }

        if (hiringStrategy.Status != HiringStrategyStatus.Approved)
        {
            return CreateResult(
                "Hiring Strategy Generated - Awaiting Approval",
                completedSteps,
                totalSteps);
        }

        completedSteps++;

        // =========================================================
        // 3. RESUME IMPORT
        // =========================================================

        if (resumeImport is null)
        {
            return CreateResult(
                "Hiring Strategy Approved - Resumes Pending",
                completedSteps,
                totalSteps);
        }

        if (resumeImport.Status == ResumeImportStatus.Queued ||
            resumeImport.Status == ResumeImportStatus.Processing)
        {
            return CreateResult(
                $"Resumes Importing " +
                $"({resumeImport.SuccessfulFiles}/" +
                $"{resumeImport.TotalFiles})",
                completedSteps,
                totalSteps);
        }

        if (resumeImport.Status == ResumeImportStatus.Failed)
        {
            return CreateResult(
                "Resume Import Failed",
                completedSteps,
                totalSteps);
        }

        if (resumeImport.Status != ResumeImportStatus.Completed)
        {
            return CreateResult(
                "Resume Import Pending",
                completedSteps,
                totalSteps);
        }

        completedSteps++;

        // =========================================================
        // 4. CANDIDATE SCREENING
        // =========================================================

        var totalCandidates = candidates.Count;

        var screenedCandidates = candidates.Count(
            x => x.LastScreenedOn.HasValue);

        var hasCandidatesScreening =
            screenedCandidates > 0;

        DateTime? candidatesScreeningCompletedOn = null;

        if (totalCandidates > 0 &&
            screenedCandidates == totalCandidates)
        {
            candidatesScreeningCompletedOn = candidates
                .Where(x => x.LastScreenedOn.HasValue)
                .Max(x => x.LastScreenedOn);
        }

        // No candidate has been screened yet.
        if (!hasCandidatesScreening)
        {
            return CreateResult(
                "Resumes Imported - Candidate Screening Pending",
                completedSteps,
                totalSteps,
                hasCandidatesScreening,
                candidatesScreeningCompletedOn);
        }

        // Some candidates screened, some still pending.
        if (!candidatesScreeningCompletedOn.HasValue)
        {
            return CreateResult(
                $"Candidate Screening In Progress " +
                $"({screenedCandidates}/{totalCandidates})",
                completedSteps,
                totalSteps,
                hasCandidatesScreening,
                candidatesScreeningCompletedOn);
        }

        // All candidates have been screened.
        completedSteps++;

        // =========================================================
        // 5. ASSESSMENT STRATEGY
        // =========================================================

        if (assessmentStrategy is null)
        {
            return CreateResult(
                "Candidate Screening Completed - " +
                "Assessment Strategy Pending",
                completedSteps,
                totalSteps,
                hasCandidatesScreening,
                candidatesScreeningCompletedOn);
        }

        if (assessmentStrategy.Status !=
            AssessmentStrategyStatus.Approved)
        {
            return CreateResult(
                "Assessment Strategy Generated - " +
                "Awaiting Approval",
                completedSteps,
                totalSteps,
                hasCandidatesScreening,
                candidatesScreeningCompletedOn);
        }

        completedSteps++;

        // =========================================================
        // 6. INVITATIONS
        // =========================================================

        var shortlistedCandidateIds = candidates
            .Where(x => x.IsRecommended)
            .Select(x => x.Id)
            .ToHashSet();

        var sentInvitationCandidateIds = emailDeliveries
            .Where(x => x.Status == EmailDeliveryStatus.Sent)
            .Select(x => x.CandidateApplicationId)
            .ToHashSet();

        var invitedShortlistedCandidates =
            shortlistedCandidateIds.Count(
                id => sentInvitationCandidateIds.Contains(id));

        var hasInvitations =
            invitedShortlistedCandidates > 0;

        DateTime? invitationsCompletedOn = null;

        if (shortlistedCandidateIds.Count > 0 &&
            invitedShortlistedCandidates ==
            shortlistedCandidateIds.Count)
        {
            invitationsCompletedOn = emailDeliveries
                .Where(x =>
                    x.Status == EmailDeliveryStatus.Sent &&
                    shortlistedCandidateIds.Contains(
                        x.CandidateApplicationId))
                .Select(x => x.SentOn)
                .Where(x => x.HasValue)
                .Max();
        }

        // No invitations sent yet.
        if (!hasInvitations)
        {
            return CreateResult(
                "Assessment Strategy Approved - " +
                "Invitations Pending",
                completedSteps,
                totalSteps,
                hasCandidatesScreening,
                candidatesScreeningCompletedOn,
                hasInvitations,
                invitationsCompletedOn);
        }

        // Some invitations sent, but not all shortlisted candidates.
        if (!invitationsCompletedOn.HasValue)
        {
            return CreateResult(
                $"Candidate Invitations In Progress " +
                $"({invitedShortlistedCandidates}/" +
                $"{shortlistedCandidateIds.Count})",
                completedSteps,
                totalSteps,
                hasCandidatesScreening,
                candidatesScreeningCompletedOn,
                hasInvitations,
                invitationsCompletedOn);
        }

        // All required invitations have been sent.
        completedSteps++;

        // =========================================================
        // READY FOR ASSESSMENT
        // =========================================================

        return CreateResult(
            "Ready for Assessment",
            completedSteps,
            totalSteps,
            hasCandidatesScreening,
            candidatesScreeningCompletedOn,
            hasInvitations,
            invitationsCompletedOn);
    }

    private static WorkflowStageResult CreateResult(
        string currentStage,
        int completedSteps,
        int totalSteps,
        bool hasCandidatesScreening = false,
        DateTime? candidatesScreeningCompletedOn = null,
        bool hasInvitations = false,
        DateTime? invitationsCompletedOn = null)
    {
        var completionPercentage =
            (completedSteps * 100) / totalSteps;

        return new WorkflowStageResult(
            currentStage,
            completionPercentage,
            hasCandidatesScreening,
            candidatesScreeningCompletedOn,
            hasInvitations,
            invitationsCompletedOn);
    }

    private sealed record CandidateWorkflowData(
        Guid Id,
        bool IsRecommended,
        DateTime? LastScreenedOn);

    private sealed record EmailDeliveryWorkflowData(
        Guid CandidateApplicationId,
        EmailDeliveryStatus Status,
        DateTime? SentOn);

    private sealed record WorkflowStageResult(
        string CurrentStage,
        int CompletionPercentage,
        bool HasCandidatesScreening,
        DateTime? CandidatesScreeningCompletedOn,
        bool HasInvitations,
        DateTime? InvitationsCompletedOn);
}