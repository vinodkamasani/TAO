using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class HiringStrategy : Entity
{
    private HiringStrategy()
    {
    }

    public HiringStrategy(
        Guid organizationId,
        Guid campaignId,
        MarkdownContent content,
        StructuredContent structuredContent)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        CampaignId = Guard.AgainstEmpty(
            campaignId,
            nameof(CampaignId));

        Content = content;
        StructuredContent = structuredContent;

        Status = HiringStrategyStatus.Generated;
        GeneratedOn = DateTime.UtcNow;
    }

    public Guid OrganizationId { get; }

    public Guid CampaignId { get; }

    public MarkdownContent Content { get; private set; }

    public StructuredContent StructuredContent { get; private set; }

    public HiringStrategyStatus Status { get; private set; }

    public DateTime GeneratedOn { get; }

    public Guid? ApprovedByUserId { get; private set; }

    public DateTime? ApprovedOn { get; private set; }

    public void Approve(Guid approvedByUserId)
    {
        if (Status == HiringStrategyStatus.Approved)
        {
            return;
        }

        ApprovedByUserId = Guard.AgainstEmpty(
            approvedByUserId,
            nameof(approvedByUserId));

        ApprovedOn = DateTime.UtcNow;
        Status = HiringStrategyStatus.Approved;
    }
}