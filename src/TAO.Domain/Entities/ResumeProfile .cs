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
        Guid resumeId,
        StructuredContent structuredContent)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        ResumeId = Guard.AgainstEmpty(
            resumeId,
            nameof(ResumeId));

        StructuredContent = structuredContent;

        GeneratedOn = DateTime.UtcNow;
    }

    public Guid OrganizationId { get; }

    public Guid ResumeId { get; }

    public StructuredContent StructuredContent { get; }

    public DateTime GeneratedOn { get; }
}