using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.Exceptions;

namespace TAO.Domain.Entities;

public sealed class Campaign : Entity
{
    private Campaign()
    {
    }

    public Campaign(
    Guid organizationId,
    string name,
    string referenceNumber,
    Guid recruiterId,
    Guid hiringManagerId,
    int numberOfOpenings)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(organizationId));

        Name = Guard.AgainstNullOrWhiteSpace(
            name,
            nameof(name));

        ReferenceNumber = Guard.AgainstNullOrWhiteSpace(
            referenceNumber,
            nameof(referenceNumber));

        RecruiterId = Guard.AgainstEmpty(
            recruiterId,
            nameof(recruiterId));

        HiringManagerId = Guard.AgainstEmpty(
            hiringManagerId,
            nameof(hiringManagerId));

        NumberOfOpenings = Guard.AgainstGreaterThanZero(
            numberOfOpenings,
            nameof(numberOfOpenings));

        Status = CampaignStatus.Ready;
    }

    public Guid OrganizationId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string ReferenceNumber { get; private set; } = string.Empty;

    public Guid RecruiterId { get; private set; }

    public Guid HiringManagerId { get; private set; }

    public int NumberOfOpenings { get; private set; }

    public CampaignStatus Status { get; private set; }

    public void ChangeName(string name)
    {
        Name = Guard.AgainstNullOrWhiteSpace(
            name,
            nameof(Name));
    }

    public void ChangeHiringManager(Guid hiringManagerId)
    {
        HiringManagerId = Guard.AgainstEmpty(
            hiringManagerId,
            nameof(HiringManagerId));
    }

    public void ChangeNumberOfOpenings(int numberOfOpenings)
    {
        NumberOfOpenings = Guard.AgainstGreaterThanZero(
            numberOfOpenings,
            nameof(NumberOfOpenings));
    }

    public void Open()
    {
        if (Status == CampaignStatus.Open)
        {
            return;
        }
        if (Status != CampaignStatus.Ready)
            throw new DomainException("Only ready campaigns can be opened.");

        Status = CampaignStatus.Open;
    }

    public void Close()
    {
        if (Status == CampaignStatus.Closed)
        {
            return;
        }
        if (Status != CampaignStatus.Open)
            throw new DomainException("Only open campaigns can be closed.");

        Status = CampaignStatus.Closed;
    }

    public void Archive()
    {
        if (Status == CampaignStatus.Archived)
        {
            return;
        }
        if (Status != CampaignStatus.Closed)
            throw new DomainException("Only closed campaigns can be archived.");

        Status = CampaignStatus.Archived;
    }
}