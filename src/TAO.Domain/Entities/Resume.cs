using TAO.Domain.Common;
using TAO.Domain.Exceptions;


namespace TAO.Domain.Entities;

public sealed class Resume : Entity
{
    private Resume()
    {
    }

    public Resume(
        Guid organizationId,
        Guid applicationId,
        string fileName,
        string contentType,
        long fileSize,
        byte[] fileContent)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        ApplicationId = Guard.AgainstEmpty(
            applicationId,
            nameof(ApplicationId));

        FileName = Guard.AgainstNullOrWhiteSpace(
            fileName,
            nameof(FileName));

        ContentType = Guard.AgainstNullOrWhiteSpace(
            contentType,
            nameof(ContentType));

        FileSize = Guard.AgainstGreaterThanZero(
            fileSize,
            nameof(FileSize));

        FileContent = fileContent
            ?? throw new DomainException("File content cannot be null.");
    }

    public Guid OrganizationId { get; }

    public Guid ApplicationId { get; }

    public string FileName { get; }

    public string ContentType { get; }

    public long FileSize { get; }

    public byte[] FileContent { get; }
}