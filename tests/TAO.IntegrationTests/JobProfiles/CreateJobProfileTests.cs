using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TAO.Api.Endpoints.JobProfiles.Create;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Infrastructure;
using TAO.Infrastructure.Persistence;
using TAO.IntegrationTests.Common;

namespace TAO.IntegrationTests.JobProfiles;

public sealed class CreateJobProfileTests
    : IntegrationTestBase
{
    public CreateJobProfileTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Create_JobProfile_When_Request_Is_Valid()
    {
        // Arrange

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaoDbContext>();

        var organization = new Organization(
            "TAO Technologies",
            "TAO");

        db.Set<Organization>().Add(organization);

        var recruiter = new User(
            organization.Id,
            "John",
            "Recruiter",
            "john@test.com",
            UserRole.Recruiter);

        var hiringManager = new User(
            organization.Id,
            "Sarah",
            "Manager",
            "sarah@test.com",
            UserRole.HiringManager);

        db.Set<User>().Add(recruiter);
        db.Set<User>().Add(hiringManager);

        await db.SaveChangesAsync();

        var campaign = Campaign.Create(
            organization.Id,
            "Senior .NET Hiring",
            "CMP-001",
            recruiter.Id,
            hiringManager.Id,
            3);

        db.Set<Campaign>().Add(campaign);

        await db.SaveChangesAsync();

        var request = new CreateJobProfileRequest(
            """
            Senior .NET Developer with 10+ years experience.

            Required Skills:
            - .NET Core
            - Azure
            - SQL

            Preferred Skills:
            - Angular
            """
        );

        // Act

        var response = await Client.PostAsJsonAsync(
            $"/api/campaigns/{campaign.Id}/job-profile",
            request);

        // Assert

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var jobProfile = await db.Set<JobProfile>()
            .SingleOrDefaultAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        jobProfile.Should().NotBeNull();

        jobProfile!.CampaignId.Should().Be(campaign.Id);

        jobProfile.OrganizationId.Should().Be(organization.Id);

        jobProfile.OriginalJobDescription.Should().Be(request.OriginalJobDescription);

        jobProfile.GeneratedContent.Should().NotBeNull();

        jobProfile.StructuredProfile.Should().NotBeNull();

        jobProfile.ProviderName.Should().Be("Fake");

        jobProfile.ModelName.Should().Be("IntegrationTest");

        jobProfile.Prompt.Should().NotBeNullOrWhiteSpace();

        jobProfile.RawResponse.Should().NotBeNullOrWhiteSpace();

        jobProfile.Status.Should().Be(JobProfileStatus.Generated);

        campaign = await db.Set<Campaign>()
            .SingleAsync(x => x.Id == campaign.Id);

    }
}