using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.Exceptions;

namespace TAO.Domain.Entities;

public sealed class CandidateInvitation : Entity
{
    private CandidateInvitation()
    {
    }

    private CandidateInvitation(
        Guid organizationId,
        Guid campaignId,
        Guid candidateApplicationId,
        DateTime? expiresOn)
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

        ExpiresOn = expiresOn;

        Status = CandidateInvitationStatus.Pending;
    }

    public Guid OrganizationId { get; private set; }

    public Guid CampaignId { get; private set; }

    public Guid CandidateApplicationId { get; private set; }

    public CandidateInvitationStatus Status { get; private set; }

    public DateTime? SentOn { get; private set; }

    public DateTime? AcceptedOn { get; private set; }

    public DateTime? ExpiresOn { get; private set; }

    public static CandidateInvitation Create(
        Guid organizationId,
        Guid campaignId,
        Guid candidateApplicationId,
        DateTime? expiresOn = null)
    {
        return new CandidateInvitation(
            organizationId,
            campaignId,
            candidateApplicationId,
            expiresOn);
    }

    public void MarkAsSent(
        DateTime sentOn)
    {
        if (Status is CandidateInvitationStatus.Accepted)
        {
            throw new DomainException(
                "An accepted candidate invitation cannot be sent again.");
        }

        if (Status is CandidateInvitationStatus.Revoked)
        {
            throw new DomainException(
                "A revoked candidate invitation cannot be sent.");
        }

        Status = CandidateInvitationStatus.Sent;
        SentOn = sentOn;

        MarkAsModified();
    }

    public void Accept(
        DateTime acceptedOn)
    {
        if (Status is CandidateInvitationStatus.Revoked)
        {
            throw new DomainException(
                "A revoked candidate invitation cannot be accepted.");
        }

        if (ExpiresOn.HasValue &&
            acceptedOn >= ExpiresOn.Value)
        {
            Status = CandidateInvitationStatus.Expired;

            MarkAsModified();

            throw new DomainException(
                "The candidate invitation has expired.");
        }

        if (Status is CandidateInvitationStatus.Accepted)
        {
            return;
        }

        Status = CandidateInvitationStatus.Accepted;
        AcceptedOn = acceptedOn;

        MarkAsModified();
    }

    public void Expire()
    {
        if (Status is CandidateInvitationStatus.Accepted)
        {
            return;
        }

        Status = CandidateInvitationStatus.Expired;

        MarkAsModified();
    }

    public void Revoke()
    {
        if (Status is CandidateInvitationStatus.Accepted)
        {
            throw new DomainException(
                "An accepted candidate invitation cannot be revoked.");
        }

        Status = CandidateInvitationStatus.Revoked;

        MarkAsModified();
    }
}