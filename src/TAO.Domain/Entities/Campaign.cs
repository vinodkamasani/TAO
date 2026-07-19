using TAO.Domain.Common;
using TAO.Domain.Enums;

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
        Guid createdByUserId,
        Guid hiringManagerUserId,
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
            createdByUserId,
            nameof(RecruiterId));

        HiringManagerUserId = Guard.AgainstEmpty(
            hiringManagerUserId,
            nameof(HiringManagerUserId));

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

    public void Rename(string name)
    {
        Name = Guard.AgainstNullOrWhiteSpace(
            name,
            nameof(Name));
    }

    public void ChangeHiringManager(Guid hiringManagerUserId)
    {
        HiringManagerUserId = Guard.AgainstEmpty(
            hiringManagerUserId,
            nameof(HiringManagerUserId));
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

        Status = CampaignStatus.Open;
    }

    public void Close()
    {
        if (Status == CampaignStatus.Closed)
        {
            return;
        }

        Status = CampaignStatus.Closed;
    }

    public void Archive()
    {
        if (Status == CampaignStatus.Archived)
        {
            return;
        }

        Status = CampaignStatus.Archived;
    }
}