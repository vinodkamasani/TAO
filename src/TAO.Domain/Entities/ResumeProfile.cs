using TAO.Domain.Common;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class ResumeProfile : Entity
{
    private ResumeProfile()
    {
    }

    public ResumeProfile(
        Guid organizationId,
        Guid applicationId,
        StructuredContent structuredContent)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        ApplicationId = Guard.AgainstEmpty(
            applicationId,
            nameof(ApplicationId));

        StructuredContent = structuredContent;

        GeneratedOn = DateTime.UtcNow;
    }

    public Guid OrganizationId { get; }

    public Guid ApplicationId { get; }

    public StructuredContent StructuredContent { get; }

    public DateTime GeneratedOn { get; }
}