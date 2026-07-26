using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.IntegrationTests.Common.TestData;

internal static class TestDataFactory
{

    private const string DefaultJobDescription =
    "Senior .NET Developer with ASP.NET Core and Angular experience.";

    private const string DefaultPrompt =
        "Generate a structured job profile.";

    private const string DefaultRawResponse =
        "{\"status\":\"success\"}";

    private const string DefaultProviderName =
        "Fake";

    private const string DefaultModelName =
        "IntegrationTest";

    private const int DefaultPromptVersion = 1;
    public static async Task<Organization> CreateOrganizationAsync(
         IApplicationDbContext context,
         string name = "Contoso",
         string code = "CONTOSO")
    {
        var organization = new Organization(
            name,
            code);

        context.Set<Organization>()
            .Add(organization);

        await context.SaveChangesAsync();

        return organization;
    }

    public static async Task<User> CreateUserAsync(
     IApplicationDbContext context,
     Guid organizationId,
     string firstName = "John",
     string lastName = "Doe",
     string? email = null,
     UserRole role = UserRole.Recruiter)
    {
        email ??= $"john.doe.{Guid.NewGuid():N}@contoso.com";

        var user = new User(
            organizationId,
            firstName,
            lastName,
            email,
            role);

        context.Set<User>()
            .Add(user);

        await context.SaveChangesAsync();

        return user;
    }

    public static async Task<Campaign> CreateCampaignAsync(
    IApplicationDbContext context,
    Guid organizationId,
    Guid recruiterId,
    Guid hiringManagerId,
    string name = "Senior .NET Developer",
    string? referenceNumber = null,
    int numberOfOpenings = 2)
    {
        referenceNumber ??= $"CMP-{Guid.NewGuid():N}";

        var campaign = Campaign.Create(
            organizationId,
            name,
            referenceNumber,
            recruiterId,
            hiringManagerId,
            numberOfOpenings);

        context.Set<Campaign>()
            .Add(campaign);

        await context.SaveChangesAsync();

        return campaign;
    }

    public static async Task<JobProfile> CreateGeneratedJobProfileAsync(
     IApplicationDbContext context,
     Guid organizationId,
     Guid campaignId,
     string? originalJobDescription = null,
     string? generatedContent = null,
     string? structuredProfile = null)
    {
        var jobProfile = JobProfile.Create(
            organizationId,
            campaignId,
            originalJobDescription ?? DefaultJobDescription,
            DefaultPrompt,
            DefaultRawResponse,
            DefaultProviderName,
            DefaultModelName,
            DefaultPromptVersion,
            new MarkdownContent(
                generatedContent ??
                "# Senior .NET Developer"),
            new StructuredContent(
                structuredProfile ??
                """
            {
              "title": "Senior .NET Developer"
            }
            """));

        context.Set<JobProfile>()
            .Add(jobProfile);

        await context.SaveChangesAsync();

        return jobProfile;
    }
}