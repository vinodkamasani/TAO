using TAO.Domain.Common;
using TAO.Domain.Exceptions;

namespace TAO.Domain.Entities;

public sealed class CandidateApplication : Entity
{
    private CandidateApplication()
    {
    }

    public CandidateApplication(
        Guid organizationId,
        Guid campaignId,
        string candidateName,
        string email,
        string phone,
        string? linkedInUrl,
        string? currentCompany,
        string? currentLocation)
    {
        OrganizationId = Guard.AgainstEmpty(
            organizationId,
            nameof(OrganizationId));

        CampaignId = Guard.AgainstEmpty(
            campaignId,
            nameof(CampaignId));

        CandidateName = Guard.AgainstNullOrWhiteSpace(
            candidateName,
            nameof(CandidateName));

        Email = Guard.AgainstNullOrWhiteSpace(
            email,
            nameof(Email));

        Phone = Guard.AgainstNullOrWhiteSpace(
            phone,
            nameof(Phone));

        LinkedInUrl = linkedInUrl?.Trim();

        CurrentCompany = currentCompany?.Trim();

        CurrentLocation = currentLocation?.Trim();

        ResumeUploadedOn = DateTime.UtcNow;
    }

    public Guid OrganizationId { get; }

    public Guid CampaignId { get; }

    public string CandidateName { get; }

    public string Email { get; }

    public string Phone { get; }

    public string? LinkedInUrl { get; }

    public string? CurrentCompany { get; }

    public string? CurrentLocation { get; }

    public byte OverallMatchPercentage { get; private set; }

    public bool IsRecommended { get; private set; }

    public DateTime ResumeUploadedOn { get; }

    public DateTime? LastScreenedOn { get; private set; }

    public void UpdateScreeningResult(
        byte overallMatchPercentage,
        bool isRecommended)
    {
        if (overallMatchPercentage > 100)
        {
            throw new DomainException(
                "Overall match percentage cannot be greater than 100.");
        }

        OverallMatchPercentage = overallMatchPercentage;
        IsRecommended = isRecommended;
        LastScreenedOn = DateTime.UtcNow;
    }
}