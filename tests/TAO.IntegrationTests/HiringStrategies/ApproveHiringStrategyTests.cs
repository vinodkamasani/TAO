using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.IntegrationTests.Common;
using TAO.IntegrationTests.Common.TestData;

namespace TAO.IntegrationTests.HiringStrategies.Approve;

public sealed class ApproveHiringStrategyTests : IntegrationTestBase
{
    public ApproveHiringStrategyTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Approve_HiringStrategy_When_Request_Is_Valid()
    {
        // Arrange
        await using var scope = Factory.Services.CreateAsyncScope();

        var context = scope.ServiceProvider
            .GetRequiredService<IApplicationDbContext>();

        var organization = await TestDataFactory.CreateOrganizationAsync(context);

        var recruiter = await TestDataFactory.CreateUserAsync(
            context,
            organization.Id);

        var hiringManager = await TestDataFactory.CreateUserAsync(
            context,
            organization.Id,
            firstName: "Jane",
            lastName: "Smith",
            role: UserRole.HiringManager);

        var campaign = await TestDataFactory.CreateCampaignAsync(
            context,
            organization.Id,
            recruiter.Id,
            hiringManager.Id);

        await TestDataFactory.CreateApprovedJobProfileAsync(
             context,
             organization.Id,
             campaign.Id,
             recruiter.Id);

        var hiringStrategy = await TestDataFactory.CreateHiringStrategyAsync(
            context,
            organization.Id,
            campaign.Id);

        var request = new
        {
            ApprovedByUserId = recruiter.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync(
                 $"/api/campaigns/{hiringStrategy.Id}/approve",
                 request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var approvedHiringStrategy = await context
            .Set<HiringStrategy>()
            .AsNoTracking()
            .SingleAsync(h => h.Id == hiringStrategy.Id);

        approvedHiringStrategy.Status
            .Should()
            .Be(HiringStrategyStatus.Approved);

        approvedHiringStrategy.ApprovedByUserId
            .Should()
            .Be(recruiter.Id);

        approvedHiringStrategy.ApprovedOn
            .Should()
            .NotBeNull();
    }
}