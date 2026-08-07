using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class JobProfile : AiGeneratedArtifact
{
    private JobProfile()
    {
    }

    private JobProfile(
        Guid organizationId,
        Guid campaignId,
        string originalJobDescription,
        string prompt,
        string rawResponse,
        string providerName,
        string modelName,
        int promptVersion,
        MarkdownContent generatedContent,
        StructuredContent structuredProfile)
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

        OriginalJobDescription = Guard.AgainstNullOrWhiteSpace(
            originalJobDescription,
            nameof(OriginalJobDescription));

        GeneratedContent = generatedContent;

        StructuredProfile = structuredProfile;

        Status = JobProfileStatus.Generated;
        GeneratedOn = DateTime.UtcNow;
    }

    public Guid OrganizationId { get; private set; }

    public Guid CampaignId { get; private set; }

    public string OriginalJobDescription { get; private set; }


    public MarkdownContent GeneratedContent { get; private set; }

    public StructuredContent StructuredProfile { get; private set; }

    public JobProfileStatus Status { get; private set; }


    public Guid? ApprovedByUserId { get; private set; }

    public DateTime? ApprovedOn { get; private set; }

    public static JobProfile Create(
        Guid organizationId,
        Guid campaignId,
        string originalJobDescription,
        string prompt,
        string rawResponse,
        string providerName,
        string modelName,
        int promptVersion,
        MarkdownContent generatedContent,
        StructuredContent structuredProfile)
    {
        return new JobProfile(
            organizationId,
            campaignId,
            originalJobDescription,
            prompt,
            rawResponse,
            providerName,
            modelName,
            promptVersion,
            generatedContent,
            structuredProfile);
    }

    public void UpdateGeneratedContent(
        MarkdownContent generatedContent,
        StructuredContent structuredProfile,
        string prompt,
        string rawResponse,
        string providerName,
        string modelName,
        int promptVersion)
    {
        if (Status == JobProfileStatus.Approved)
        {
            throw new InvalidOperationException(
                "Approved Job Profile cannot be modified.");
        }

        GeneratedContent = generatedContent;

        StructuredProfile = structuredProfile;

        Prompt = Guard.AgainstNullOrWhiteSpace(
            prompt,
            nameof(prompt));

        RawResponse = Guard.AgainstNullOrWhiteSpace(
            rawResponse,
            nameof(rawResponse));

        ProviderName = Guard.AgainstNullOrWhiteSpace(
            providerName,
            nameof(providerName));

        ModelName = Guard.AgainstNullOrWhiteSpace(
            modelName,
            nameof(modelName));

        PromptVersion = Guard.AgainstGreaterThanZero(
            promptVersion,
            nameof(promptVersion));

        GeneratedOn = DateTime.UtcNow;

        MarkAsModified();
    }

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

        MarkAsModified();
    }
}