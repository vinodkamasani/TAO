using TAO.Domain.Common;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class ResumeScreening : Entity
{
    private ResumeScreening()
    {
    }

    public ResumeScreening(
        Guid organizationId,
        Guid resumeId,
        MarkdownContent content,
        StructuredContent structuredContent)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        ResumeId = Guard.AgainstEmpty(
            resumeId,
            nameof(ResumeId));

        Content = content;

        StructuredContent = structuredContent;

        GeneratedOn = DateTime.UtcNow;
    }

    public Guid OrganizationId { get; }

    public Guid ResumeId { get; }

    public MarkdownContent Content { get; }

    public StructuredContent StructuredContent { get; }

    public DateTime GeneratedOn { get; }
}