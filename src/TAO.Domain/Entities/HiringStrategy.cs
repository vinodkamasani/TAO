using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

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


    public void UpdateResumeMatchThreshold(int thresholdPercentage)
    {
        if (thresholdPercentage is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(thresholdPercentage),
                "Resume match threshold must be between 0 and 100.");
        }

        // Update the existing structured JSON content.
        var structuredJson = JsonNode.Parse(StructuredContent.Value)
            as JsonObject
            ?? throw new InvalidOperationException(
                "Hiring strategy structured content must be a JSON object.");

        if (!structuredJson.ContainsKey("recommendedResumeMatchThreshold"))
        {
            throw new InvalidOperationException(
                "The recommended resume match threshold was not found in structured content.");
        }

        structuredJson["recommendedResumeMatchThreshold"] =
            thresholdPercentage;

        var updatedStructuredContent = StructuredContent.Create(
            structuredJson.ToJsonString());

        // Update the existing Markdown section.
        var thresholdValue = thresholdPercentage.ToString(
            CultureInfo.InvariantCulture);

        const string pattern =
            @"(?m)^(## Resume Match Threshold)[ \t]*(\r?\n)[^\r\n]*";

        var updatedMarkdown = Regex.Replace(
            Content.Value,
            pattern,
            match => $"{match.Groups[1].Value}" +
                     $"{match.Groups[2].Value}" +
                     thresholdValue,
            RegexOptions.None);

        if (updatedMarkdown == Content.Value)
        {
            throw new InvalidOperationException(
                "The Resume Match Threshold section was not found in Markdown content.");
        }

        // Assign both value objects only after both updates succeed.
        StructuredContent = updatedStructuredContent;
        Content = MarkdownContent.Create(updatedMarkdown);
    }

}