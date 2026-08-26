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

    private const string DefaultHiringStrategyPrompt = "Generate Hiring Strategy";

 
    public static async Task<Organization> CreateOrganizationAsync(
         IApplicationDbContext context,
         string name = "Contoso",
         string code = "CONTOSO")
    {
        var organization = new Organization(
            name + $"{Guid.NewGuid()}",
            code + $"{Guid.NewGuid()}");

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
      string? prompt = null,
      string? rawResponse = null,
      string? providerName = null,
      string? modelName = null,
      int? promptVersion = null,
      string? generatedContent = null,
      string? structuredProfile = null)
    {
        var jobProfile = JobProfile.Create(
            organizationId,
            campaignId,
            originalJobDescription ?? DefaultJobDescription,
            prompt ?? DefaultPrompt,
            rawResponse ?? DefaultRawResponse,
            providerName ?? DefaultProviderName,
            modelName ?? DefaultModelName,
            promptVersion ?? DefaultPromptVersion,
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


    public static async Task<JobProfile> CreateApprovedJobProfileAsync(
     IApplicationDbContext context,
     Guid organizationId,
     Guid campaignId,
     Guid approvedByUserId,
     string? originalJobDescription = null,
     string? prompt = null,
     string? rawResponse = null,
     string? providerName = null,
     string? modelName = null,
     int? promptVersion = null,
     string? generatedContent = null,
     string? structuredProfile = null)
    {
        var jobProfile = await CreateGeneratedJobProfileAsync(
            context,
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

        jobProfile.Approve(approvedByUserId);

        await context.SaveChangesAsync();

        return jobProfile;
    }

    public static async Task<HiringStrategy> CreateHiringStrategyAsync(
    IApplicationDbContext context,
    Guid organizationId,
    Guid campaignId,
    string? prompt = null,
    string? rawResponse = null,
    string? providerName = null,
    string? modelName = null,
    int? promptVersion = null,
    string? generatedContent = null,
    string? structuredContent = null)
    {
        var hiringStrategy = HiringStrategy.Create(
            organizationId,
            campaignId,
            prompt ?? DefaultHiringStrategyPrompt,
            rawResponse ?? DefaultRawResponse,
            providerName ?? DefaultProviderName,
            modelName ?? DefaultModelName,
            promptVersion ?? DefaultPromptVersion,
            new MarkdownContent(
                generatedContent ??
                """
            # Hiring Strategy

            Recommended resume match threshold: **80%**
            """),
            new StructuredContent(
                structuredContent ??
                """
            {
              "minimumExperienceYears": 5,
              "recommendedResumeMatchThreshold": 80,
              "requiredSkills": [
                {
                  "name": "C#"
                }
              ],
              "preferredSkills": [
                {
                  "name": "Azure"
                }
              ],
              "recruiterGuidance": [
                "Prioritize candidates with recent .NET experience."
              ]
            }
            """));

        context.Set<HiringStrategy>()
            .Add(hiringStrategy);

        await context.SaveChangesAsync();

        return hiringStrategy;
    }

    public static async Task<HiringStrategy> CreateApprovedHiringStrategyAsync(
    IApplicationDbContext context,
    Guid organizationId,
    Guid campaignId,
    Guid approvedByUserId,
    string? prompt = null,
    string? rawResponse = null,
    string? providerName = null,
    string? modelName = null,
    int? promptVersion = null,
    string? generatedContent = null,
    string? structuredContent = null)
    {
        var hiringStrategy = await CreateHiringStrategyAsync(
            context,
            organizationId,
            campaignId,
            prompt,
            rawResponse,
            providerName,
            modelName,
            promptVersion,
            generatedContent,
            structuredContent);

        hiringStrategy.Approve(
            approvedByUserId);

        await context.SaveChangesAsync();

        return hiringStrategy;
    }

    public static async Task<AssessmentStrategy> CreateAssessmentStrategyAsync(
        IApplicationDbContext context,
        Guid organizationId,
        Guid campaignId,
        string? assessmentName = null,
        string? prompt = null,
        string? rawResponse = null,
        string? providerName = null,
        string? modelName = null,
        int? promptVersion = null,
        string? generatedContent = null,
        string? structuredContent = null)
    {
        var assessmentStrategy = new AssessmentStrategy(
            organizationId,
            campaignId,
            assessmentName ?? "Senior .NET Developer Assessment",
            new MarkdownContent(
                generatedContent ??
                """
            # Senior .NET Developer Assessment

            ## Assessment Overview

            **Total Rounds:** 1

            **Total Duration:** 60 minutes

            ## Assessment Rounds

            ### Round 1: Coding

            - **Difficulty:** Medium
            - **Duration:** 60 minutes
            - **Questions:** 3

            **Competencies:**

            - **C#:** High priority
            - **ASP.NET Core:** High priority
            """),
            new StructuredContent(
                structuredContent ??
                """
            {
              "assessmentName": "Senior .NET Developer Assessment",
              "rounds": [
                {
                  "order": 1,
                  "type": "Coding",
                  "difficulty": "Medium",
                  "durationInMinutes": 60,
                  "questionCount": 3,
                  "competencies": [
                    {
                      "name": "C#",
                      "priority": "High"
                    },
                    {
                      "name": "ASP.NET Core",
                      "priority": "High"
                    }
                  ]
                }
              ]
            }
            """),
            prompt ?? "Generate Assessment Strategy",
            rawResponse ??
                """
            {
              "assessmentName": "Senior .NET Developer Assessment",
              "rounds": [
                {
                  "order": 1,
                  "type": "Coding",
                  "difficulty": "Medium",
                  "durationInMinutes": 60,
                  "questionCount": 3,
                  "competencies": [
                    {
                      "name": "C#",
                      "priority": "High"
                    }
                  ]
                }
              ]
            }
            """,
            providerName ?? DefaultProviderName,
            modelName ?? DefaultModelName,
            promptVersion ?? DefaultPromptVersion);

        context.Set<AssessmentStrategy>()
            .Add(assessmentStrategy);

        await context.SaveChangesAsync();

        var round = AssessmentRound.Create(
            assessmentStrategy.Id,
            1,
            AssessmentRoundType.Coding,
            AssessmentDifficulty.Medium,
            60,
            3,
            new List<AssessmentRoundCompetency>
            {
            new()
            {
                Name = "C#",
                Priority = "High",
                MinimumPassPercentage = 70
            },
            new()
            {
                Name = "ASP.NET Core",
                Priority = "High",
                MinimumPassPercentage = 70
            }
            });

        context.Set<AssessmentRound>()
            .Add(round);

        await context.SaveChangesAsync();

        return assessmentStrategy;
    }
}