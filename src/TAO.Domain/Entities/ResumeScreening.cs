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
        Guid applicationId,
        MarkdownContent content,
        StructuredContent structuredContent)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        ApplicationId = Guard.AgainstEmpty(
            applicationId,
            nameof(ApplicationId));

        Content = content;

        StructuredContent = structuredContent;

        GeneratedOn = DateTime.UtcNow;
    }

    public static ResumeScreening Create(
        Guid organizationId,
        Guid applicationId,
        MarkdownContent content,
        StructuredContent structuredContent)
    {
        Guard.AgainstEmpty(organizationId, nameof(OrganizationId));
        Guard.AgainstEmpty(applicationId, nameof(ApplicationId));
        Guard.AgainstNull(content, nameof(Content));
        Guard.AgainstNull(structuredContent, nameof(StructuredContent));

        return new ResumeScreening(
            organizationId,
            applicationId,
            content,
            structuredContent);
    }

    public Guid OrganizationId { get; }

    public Guid ApplicationId { get; }

    public MarkdownContent Content { get; }

    public StructuredContent StructuredContent { get; }

    public DateTime GeneratedOn { get; }

}