using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Infrastructure;
using TAO.Infrastructure.Persistence;
using TAO.IntegrationTests.Common;
using TAO.IntegrationTests.Common.TestData;

namespace TAO.IntegrationTests.HiringStrategies;

public sealed class CreateHiringStrategyTests
    : IntegrationTestBase
{
    public CreateHiringStrategyTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Create_HiringStrategy_When_Request_Is_Valid()
    {
        // Arrange

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaoDbContext>();

        var organization = await TestDataFactory.CreateOrganizationAsync(db);

        var recruiter = await TestDataFactory.CreateUserAsync(
            db,
            organization.Id);

        var hiringManager = await TestDataFactory.CreateUserAsync(
            db,
            organization.Id,
            firstName: "Sarah",
            lastName: "Manager",
            role: UserRole.HiringManager);

        var campaign = await TestDataFactory.CreateCampaignAsync(
            db,
            organization.Id,
            recruiter.Id,
            hiringManager.Id);

        await TestDataFactory.CreateApprovedJobProfileAsync(
              db,
              organization.Id,
              campaign.Id,
              recruiter.Id);


        // Act

        var response = await Client.PostAsync(
            $"/api/campaigns/{campaign.Id}/hiring-strategy",
            null);

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine(body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var hiringStrategy = await db.Set<HiringStrategy>()
     .SingleAsync(
         hs => hs.CampaignId == campaign.Id);

        hiringStrategy.OrganizationId.Should().Be(organization.Id);

        hiringStrategy.Should().NotBeNull();

        hiringStrategy!.CampaignId.Should().Be(campaign.Id);

        hiringStrategy.OrganizationId.Should().Be(organization.Id);

        hiringStrategy.Content.Should().NotBeNull();

        hiringStrategy.StructuredContent.Should().NotBeNull();

        hiringStrategy.ProviderName.Should().Be("Fake");

        hiringStrategy.ModelName.Should().Be("IntegrationTest");

        hiringStrategy.Prompt.Should().NotBeNullOrWhiteSpace();

        hiringStrategy.RawResponse.Should().NotBeNullOrWhiteSpace();

        hiringStrategy.Status.Should().Be(HiringStrategyStatus.Generated);
    }
}