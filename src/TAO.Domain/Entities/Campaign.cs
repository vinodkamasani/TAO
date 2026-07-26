using TAO.Domain.Common;
using TAO.Domain.Enums;

namespace TAO.Domain.Entities;

public sealed class Campaign : Entity
{
    private Campaign()
    {
    }

<<<<<<< Updated upstream
    public Campaign(
        Guid organizationId,
        string name,
        string referenceNumber,
        Guid createdByUserId,
        Guid hiringManagerUserId,
=======
    private Campaign(
        Guid organizationId,
        string name,
        string referenceNumber,
        Guid recruiterId,
        Guid hiringManagerId,
>>>>>>> Stashed changes
        int numberOfOpenings)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        Name = Guard.AgainstNullOrWhiteSpace(
            name,
            nameof(Name));

        ReferenceNumber = Guard.AgainstNullOrWhiteSpace(
            referenceNumber,
            nameof(ReferenceNumber));

        RecruiterId = Guard.AgainstEmpty(
<<<<<<< Updated upstream
            createdByUserId,
            nameof(RecruiterId));

        HiringManagerUserId = Guard.AgainstEmpty(
            hiringManagerUserId,
            nameof(HiringManagerUserId));
=======
            recruiterId,
            nameof(RecruiterId));

        HiringManagerId = Guard.AgainstEmpty(
            hiringManagerId,
            nameof(HiringManagerId));
>>>>>>> Stashed changes

        NumberOfOpenings = Guard.AgainstGreaterThanZero(
            numberOfOpenings,
            nameof(NumberOfOpenings));

        Status = CampaignStatus.Ready;
    }

    public Guid OrganizationId { get; }

    public string Name { get; private set; } = string.Empty;

    public string ReferenceNumber { get; }

    public Guid RecruiterId { get; }

    public Guid HiringManagerUserId { get; private set; }

    public int NumberOfOpenings { get; private set; }

    public CampaignStatus Status { get; private set; }

<<<<<<< Updated upstream
    public void Rename(string name)
=======
    public static Campaign Create(
        Guid organizationId,
        string name,
        string referenceNumber,
        Guid recruiterId,
        Guid hiringManagerId,
        int numberOfOpenings)
    {
        return new Campaign(
            organizationId,
            name,
            referenceNumber,
            recruiterId,
            hiringManagerId,
            numberOfOpenings);
    }

    public void ChangeName(string name)
>>>>>>> Stashed changes
    {
        Name = Guard.AgainstNullOrWhiteSpace(
            name,
            nameof(Name));

        MarkAsModified();
    }

    public void ChangeHiringManager(Guid hiringManagerUserId)
    {
<<<<<<< Updated upstream
        HiringManagerUserId = Guard.AgainstEmpty(
            hiringManagerUserId,
            nameof(HiringManagerUserId));
=======
        HiringManagerId = Guard.AgainstEmpty(
            hiringManagerId,
            nameof(HiringManagerId));

        MarkAsModified();
>>>>>>> Stashed changes
    }

    public void ChangeNumberOfOpenings(int numberOfOpenings)
    {
        NumberOfOpenings = Guard.AgainstGreaterThanZero(
            numberOfOpenings,
            nameof(NumberOfOpenings));

        MarkAsModified();
    }

    public void Open()
    {
        if (Status == CampaignStatus.Open)
        {
            return;
        }
<<<<<<< Updated upstream
=======

        if (Status != CampaignStatus.Ready)
        {
            throw new DomainException(
                "Only ready campaigns can be opened.");
        }
>>>>>>> Stashed changes

        Status = CampaignStatus.Open;

        MarkAsModified();
    }

    public void Close()
    {
        if (Status == CampaignStatus.Closed)
        {
            return;
        }
<<<<<<< Updated upstream
=======

        if (Status != CampaignStatus.Open)
        {
            throw new DomainException(
                "Only open campaigns can be closed.");
        }
>>>>>>> Stashed changes

        Status = CampaignStatus.Closed;

        MarkAsModified();
    }

    public void Archive()
    {
        if (Status == CampaignStatus.Archived)
        {
            return;
        }
<<<<<<< Updated upstream
=======

        if (Status != CampaignStatus.Closed)
        {
            throw new DomainException(
                "Only closed campaigns can be archived.");
        }
>>>>>>> Stashed changes

        Status = CampaignStatus.Archived;

        MarkAsModified();
    }
}