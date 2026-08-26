using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Infrastructure;
using TAO.IntegrationTests.Common;
using TAO.IntegrationTests.Common.TestData;

namespace TAO.IntegrationTests.AssessmentStrategies;

public sealed class ApproveAssessmentStrategyTests
    : IntegrationTestBase
{
    public ApproveAssessmentStrategyTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Approve_AssessmentStrategy_When_Request_Is_Valid()
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
            firstName: "Jane",
            lastName: "Smith",
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

        await TestDataFactory.CreateApprovedHiringStrategyAsync(
            db,
            organization.Id,
            campaign.Id,
            recruiter.Id);

        var assessmentStrategy =
            await TestDataFactory.CreateAssessmentStrategyAsync(
                db,
                organization.Id,
                campaign.Id);

        var request = new
        {
            ApprovedByUserId = recruiter.Id
        };

        // Act

        var response = await Client.PostAsJsonAsync(
            $"/api/campaigns/assessment-strategies/{assessmentStrategy.Id}/approve",
            request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        var approvedAssessmentStrategy = await db
            .Set<AssessmentStrategy>()
            .AsNoTracking()
            .SingleAsync(
                x => x.Id == assessmentStrategy.Id);

        approvedAssessmentStrategy.Status
            .Should()
            .Be(AssessmentStrategyStatus.Approved);

        approvedAssessmentStrategy.ApprovedByUserId
            .Should()
            .Be(recruiter.Id);

        approvedAssessmentStrategy.ApprovedOn
            .Should()
            .NotBeNull();
    }
}