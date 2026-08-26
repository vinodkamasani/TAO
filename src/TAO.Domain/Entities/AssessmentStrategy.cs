using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class AssessmentStrategy : AiGeneratedArtifact
{
    private AssessmentStrategy()
    {
    }

    public AssessmentStrategy(
     Guid organizationId,
     Guid campaignId,
     string assessmentName,
     MarkdownContent content,
     StructuredContent structuredContent,
     string prompt,
     string rawResponse,
     string providerName,
     string modelName,
     int promptVersion)
     : base(
         prompt,
         rawResponse,
         providerName,
         modelName,
         promptVersion)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        CampaignId = Guard.AgainstEmpty(
            campaignId,
            nameof(CampaignId));

        AssessmentName = Guard.AgainstNullOrWhiteSpace(
            assessmentName,
            nameof(AssessmentName));

        Content = content;
        StructuredContent = structuredContent;

        Status = AssessmentStrategyStatus.Generated;
    }

    public Guid OrganizationId { get; }

    public Guid CampaignId { get; }

    public string AssessmentName { get; private set; }

    public MarkdownContent Content { get; private set; }

    public StructuredContent StructuredContent { get; private set; }

    public AssessmentStrategyStatus Status { get; private set; }

    public DateTime GeneratedOn { get; }

    public Guid? ApprovedByUserId { get; private set; }

    public DateTime? ApprovedOn { get; private set; }

    public void Update(
        string assessmentName,
        MarkdownContent content,
        StructuredContent structuredContent)
    {
        if (Status == AssessmentStrategyStatus.Approved)
        {
            throw new InvalidOperationException(
                "Approved Assessment Strategy cannot be modified.");
        }

        AssessmentName = Guard.AgainstNullOrWhiteSpace(
            assessmentName,
            nameof(AssessmentName));

        Content = content;
        StructuredContent = structuredContent;
    }

    public void Approve(Guid approvedByUserId)
    {
        if (Status == AssessmentStrategyStatus.Approved)
        {
            return;
        }

        ApprovedByUserId = Guard.AgainstEmpty(
            approvedByUserId,
            nameof(approvedByUserId));

        ApprovedOn = DateTime.UtcNow;
        Status = AssessmentStrategyStatus.Approved;
    }
}