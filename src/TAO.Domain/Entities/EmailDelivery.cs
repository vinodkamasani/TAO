using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.Exceptions;

namespace TAO.Domain.Entities;

public sealed class EmailDelivery : Entity
{
    private EmailDelivery()
    {
    }

    private EmailDelivery(
        Guid organizationId,
        Guid campaignId,
        Guid candidateApplicationId,
        string recipientEmail,
        string subject,
        string body)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        CampaignId = Guard.AgainstEmpty(
            campaignId,
            nameof(CampaignId));

        CandidateApplicationId = Guard.AgainstEmpty(
            candidateApplicationId,
            nameof(CandidateApplicationId));

        RecipientEmail = Guard.AgainstNullOrWhiteSpace(
            recipientEmail,
            nameof(RecipientEmail));

        Subject = Guard.AgainstNullOrWhiteSpace(
            subject,
            nameof(Subject));

        Body = Guard.AgainstNullOrWhiteSpace(
            body,
            nameof(Body));

        Status = EmailDeliveryStatus.Pending;
    }

    public Guid OrganizationId { get; private set; }

    public Guid CampaignId { get; private set; }

    public Guid CandidateApplicationId { get; private set; }

    public string RecipientEmail { get; private set; } = string.Empty;

    public string Subject { get; private set; } = string.Empty;

    public string Body { get; private set; } = string.Empty;

    public EmailDeliveryStatus Status { get; private set; }

    public DateTime? SentOn { get; private set; }

    public DateTime? FailedOn { get; private set; }

    public string? FailureReason { get; private set; }

    public static EmailDelivery Create(
        Guid organizationId,
        Guid campaignId,
        Guid candidateApplicationId,
        string recipientEmail,
        string subject,
        string body)
    {
        return new EmailDelivery(
            organizationId,
            campaignId,
            candidateApplicationId,
            recipientEmail,
            subject,
            body);
    }

    public void MarkAsSent(DateTime sentOn)
    {
        Status = EmailDeliveryStatus.Sent;
        SentOn = sentOn;
        FailedOn = null;
        FailureReason = null;

        MarkAsModified();
    }

    public void MarkAsFailed(
        string failureReason,
        DateTime failedOn)
    {
        FailureReason = Guard.AgainstNullOrWhiteSpace(
            failureReason,
            nameof(FailureReason));

        Status = EmailDeliveryStatus.Failed;
        FailedOn = failedOn;

        MarkAsModified();
    }
}