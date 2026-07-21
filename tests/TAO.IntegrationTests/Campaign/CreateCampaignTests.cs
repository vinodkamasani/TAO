using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TAO.Api.Endpoints.Campaigns.Create;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Infrastructure;
using TAO.Infrastructure.Persistence;
using TAO.IntegrationTests.Common;

namespace TAO.IntegrationTests.Campaigns;

public sealed class CreateCampaignTests
    : IntegrationTestBase
{
    public CreateCampaignTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateCampaign_Should_CreateCampaign_When_RequestIsValid()
    {
        // Arrange

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<TaoDbContext>();

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

        var request = new CreateCampaignRequest(
            organization.Id,
            "Senior .NET Hiring",
            "CMP-001",
            recruiter.Id,
            hiringManager.Id,
            3);

        // Act

        var response = await Client.PostAsJsonAsync(
            "/api/campaigns",
            request);

        // Assert

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var campaign = await db
            .Set<Campaign>()
            .SingleOrDefaultAsync(x =>
                x.ReferenceNumber == "CMP-001");

        Assert.NotNull(campaign);

        Assert.Equal(
            CampaignStatus.Ready,
            campaign!.Status);

        Assert.Equal(
            recruiter.Id,
            campaign.RecruiterId);

        Assert.Equal(
            hiringManager.Id,
            campaign.HiringManagerId);
    }
}