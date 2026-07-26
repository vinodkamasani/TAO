using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TAO.Application.JobProfiles.Get;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using TAO.Infrastructure;
using TAO.Infrastructure.Persistence;
using TAO.IntegrationTests.Common;

namespace TAO.IntegrationTests.JobProfiles;

public sealed class GetJobProfileTests
    : IntegrationTestBase
{
    public GetJobProfileTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Return_JobProfile_When_Id_Exists()
    {
        // Arrange

        var db = Services.GetRequiredService<TaoDbContext>();

        var unique = Guid.NewGuid().ToString("N");

        var organization = new Organization(
            $"TAO Org {unique}",
            $"TAO-{unique}");

        db.Set<Organization>().Add(organization);

        var recruiter = new User(
                    organization.Id,
                    "John",
                    "Recruiter",
                    $"john-{unique}@tao.test",
                    UserRole.Recruiter);

        var hiringManager = new User(
                    organization.Id,
                    "Sarah",
                    "Manager",
                    $"sarah-{unique}@tao.test",
                    UserRole.HiringManager);

        db.Set<User>().Add(recruiter);
        db.Set<User>().Add(hiringManager);

        await db.SaveChangesAsync();

        var campaign = Campaign.Create(
                     organization.Id,
                     $"Campaign {unique}",
                     $"CMP-{unique}",
                     recruiter.Id,
                     hiringManager.Id,
                     3);

        db.Set<Campaign>().Add(campaign);

        await db.SaveChangesAsync();

        var jobProfile = JobProfile.Create(
            organization.Id,
            campaign.Id,
            "Original JD",
            "Prompt",
            "Raw Response",
            "Fake",
            "IntegrationTest",
            1,
            new MarkdownContent("# Senior .NET Developer"),
            new StructuredContent("{\"role\":\"Senior .NET Developer\"}"));

        db.Set<JobProfile>().Add(jobProfile);

        await db.SaveChangesAsync();

        // Act

        var response = await Client.GetAsync(
            $"/api/jobprofiles/{jobProfile.Id}");

        // Assert

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<JobProfileResponse>();

        result.Should().NotBeNull();

        result!.Id.Should().Be(jobProfile.Id);

        result.CampaignId.Should().Be(campaign.Id);

        result.OriginalJobDescription
            .Should()
            .Be("Original JD");

        result.GeneratedContent
            .Should()
            .Be("# Senior .NET Developer");

        result.StructuredProfile
            .Should()
            .Be("{\"role\":\"Senior .NET Developer\"}");

        result.Status.Should()
            .Be(JobProfileStatus.Generated);
    }
}