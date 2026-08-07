using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

public sealed class HiringStrategy : AiGeneratedArtifact
{
    private HiringStrategy()
    {
    }

    public HiringStrategy(
        Guid organizationId,
        Guid campaignId,
        string prompt,
        string rawResponse,
        string providerName,
        string modelName,
        int promptVersion,
        MarkdownContent content,
        StructuredContent structuredContent)
        : base(
            prompt,
            rawResponse,
            providerName,
            modelName,
            promptVersion)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(organizationId));

        CampaignId = Guard.AgainstEmpty(
            campaignId,
            nameof(campaignId));

        Content = content;
        StructuredContent = structuredContent;

        Status = HiringStrategyStatus.Generated;
    }

    public Guid OrganizationId { get; }

    public Guid CampaignId { get; }

    public MarkdownContent Content { get; private set; }

    public StructuredContent StructuredContent { get; private set; }

    public HiringStrategyStatus Status { get; private set; }

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
    public static HiringStrategy Create(
    Guid organizationId,
    Guid campaignId,
    string prompt,
    string rawResponse,
    string providerName,
    string modelName,
    int promptVersion,
    MarkdownContent content,
    StructuredContent structuredContent)
    {
        return new HiringStrategy(
            organizationId,
            campaignId,
            prompt,
            rawResponse,
            providerName,
            modelName,
            promptVersion,
            content,
            structuredContent);
    }
}