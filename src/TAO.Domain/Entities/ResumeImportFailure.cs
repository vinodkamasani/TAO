using TAO.Domain.Common;

namespace TAO.Domain.Entities;

public sealed class ResumeImportFailure : Entity
{
    private ResumeImportFailure()
    {
    }

    public ResumeImportFailure(
        Guid organizationId,
        Guid resumeImportId,
        string fileName,
        string failureReason)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        ResumeImportId = Guard.AgainstEmpty(
            resumeImportId,
            nameof(ResumeImportId));

        FileName = Guard.AgainstNullOrWhiteSpace(
            fileName,
            nameof(FileName));

        FailureReason = Guard.AgainstNullOrWhiteSpace(
            failureReason,
            nameof(FailureReason));
    }

    public Guid OrganizationId { get; }

    public Guid ResumeImportId { get; }

    public string FileName { get; }

    public string FailureReason { get; }
}