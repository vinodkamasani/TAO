using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TAO.Application.HiringStrategies.Get;
using TAO.Domain.Enums;
using TAO.Infrastructure;
using TAO.IntegrationTests.Common;
using TAO.IntegrationTests.Common.TestData;
using Xunit;

namespace TAO.IntegrationTests.HiringStrategies;

public sealed class GetHiringStrategyTests : IntegrationTestBase
{
    public GetHiringStrategyTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Return_HiringStrategy_When_Exists()
    {
        // Arrange
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaoDbContext>();

        var organization = await TestDataFactory.CreateOrganizationAsync(db);


        var user = await TestDataFactory.CreateUserAsync(
            db,
            organization.Id);

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


        var hiringStrategy = await TestDataFactory.CreateHiringStrategyAsync(
            db,
            organization.Id,
            campaign.Id
            );

        // Act

        var response = await Client.GetAsync(
            $"api/campaigns/{campaign.Id}/hiring-strategy");

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GetHiringStrategyResponse>();

        result.Should().NotBeNull();

        result!.Id.Should().Be(hiringStrategy.Id);
        result.CampaignId.Should().Be(campaign.Id);
        result.GeneratedContent.Should().NotBeNullOrWhiteSpace();
        result.StructuredContent.Should().NotBeNullOrWhiteSpace();
        result.ProviderName.Should().NotBeNullOrWhiteSpace();
        result.ModelName.Should().NotBeNullOrWhiteSpace();
        result.PromptVersion.Should().BeGreaterThan(0);
        result.Status.Should().Be(hiringStrategy.Status);
    }

    //[Fact]
    //public async Task Should_Return_NotFound_When_HiringStrategy_Does_Not_Exist()
    //{
    //    // Arrange

    //    var organization = await TestDataFactory.CreateOrganizationAsync(
    //        Services);

    //    var user = await TestDataFactory.CreateUserAsync(
    //        Services,
    //        organization.Id);

    //    var campaign = await TestDataFactory.CreateCampaignAsync(
    //        Services,
    //        organization.Id,
    //        user.Id);

    //    // Act

    //    var response = await Client.GetAsync(
    //        $"api/campaigns/{campaign.Id}/hiring-strategy");

    //    // Assert

    //    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    //}
}