namespace TAO.Application.Campaigns.Get;

public sealed record CampaignWorkflowStateResponse(
    Guid CampaignId,
    string CampaignName,
    string CurrentStage,
    int CompletionPercentage,

    // Job Profile Stage
    bool HasJobProfile,
    string? JobProfileStatus,
    DateTime? JobProfileCreatedOn,
    DateTime? JobProfileApprovedOn,

    // Hiring Strategy Stage
    bool HasHiringStrategy,
    string? HiringStrategyStatus,
    DateTime? HiringStrategyCreatedOn,
    DateTime? HiringStrategyApprovedOn,

    // Assessment Strategy Stage
    bool HasAssessmentStrategy,
    string? AssessmentStrategyStatus,
    DateTime? AssessmentStrategyCreatedOn,
    DateTime? AssessmentStrategyApprovedOn,

    // Resume Import Stage
    bool HasResumeImport,
    string? ResumeImportStatus,
    int TotalResumes,
    int SuccessfulResumes,
    int FailedResumes,
    DateTime? ResumeImportCompletedOn);
