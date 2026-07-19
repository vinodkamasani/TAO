using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class JobProfile : Entity
{
    private JobProfile()
    {
    }

    public JobProfile(
        Guid organizationId,
        Guid campaignId,
        string originalJobDescription,
        MarkdownContent content,
        StructuredContent structuredContent)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        CampaignId = Guard.AgainstEmpty(
            campaignId,
            nameof(CampaignId));

        OriginalJobDescription = Guard.AgainstNullOrWhiteSpace(
            originalJobDescription,
            nameof(OriginalJobDescription));

        Content = content;
        StructuredContent = structuredContent;

        Status = JobProfileStatus.Generated;
        GeneratedOn = DateTime.UtcNow;
    }

    public Guid OrganizationId { get; }

    public Guid CampaignId { get; }

    public string OriginalJobDescription { get; }

    public MarkdownContent Content { get; private set; }

    public StructuredContent StructuredContent { get; private set; }

    public JobProfileStatus Status { get; private set; }

    public DateTime GeneratedOn { get; }

    public Guid? ApprovedByUserId { get; private set; }

    public DateTime? ApprovedOn { get; private set; }

    public void Approve(Guid approvedByUserId)
    {
        if (Status == JobProfileStatus.Approved)
        {
            return;
        }

        ApprovedByUserId = Guard.AgainstEmpty(
            approvedByUserId,
            nameof(ApprovedByUserId));

        ApprovedOn = DateTime.UtcNow;
        Status = JobProfileStatus.Approved;
    }
}