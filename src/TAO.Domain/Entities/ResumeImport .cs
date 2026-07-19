using TAO.Domain.Common;
using TAO.Domain.Enums;

namespace TAO.Domain.Entities;

public sealed class ResumeImport : Entity
{
    private ResumeImport()
    {
    }

    public ResumeImport(
        Guid organizationId,
        Guid campaignId,
        int totalFiles)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        CampaignId = Guard.AgainstEmpty(
            campaignId,
            nameof(CampaignId));

        TotalFiles = Guard.AgainstGreaterThanZero(
            totalFiles,
            nameof(TotalFiles));

        Status = ResumeImportStatus.Queued;
    }

    public Guid OrganizationId { get; }

    public Guid CampaignId { get; }

    public ResumeImportStatus Status { get; private set; }

    public int TotalFiles { get; }

    public int SuccessfulFiles { get; private set; }

    public int FailedFiles { get; private set; }

    public void MarkProcessing()
    {
        Status = ResumeImportStatus.Processing;
    }

    public void RecordSuccess()
    {
        SuccessfulFiles++;
    }

    public void RecordFailure()
    {
        FailedFiles++;
    }

    public void Complete()
    {
        Status = ResumeImportStatus.Completed;
    }

    public void Fail()
    {
        Status = ResumeImportStatus.Failed;
    }
}