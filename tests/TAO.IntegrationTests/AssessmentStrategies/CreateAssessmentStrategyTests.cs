using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Infrastructure;
using TAO.IntegrationTests.Common;
using TAO.IntegrationTests.Common.TestData;

namespace TAO.IntegrationTests.AssessmentStrategies;

public sealed class CreateAssessmentStrategyTests
    : IntegrationTestBase
{
    public CreateAssessmentStrategyTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Create_AssessmentStrategy_When_Request_Is_Valid()
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

        await TestDataFactory.CreateApprovedHiringStrategyAsync(
            db,
            organization.Id,
            campaign.Id,
            recruiter.Id);

        // Act

        var response = await Client.PostAsync(
            $"/api/campaigns/{campaign.Id}/assessment-strategy",
            null);

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine(body);

        // Assert

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var assessmentStrategy = await db
            .Set<AssessmentStrategy>()
            .SingleAsync(
                x => x.CampaignId == campaign.Id);

        assessmentStrategy.Should().NotBeNull();

        assessmentStrategy.OrganizationId
            .Should()
            .Be(organization.Id);

        assessmentStrategy.CampaignId
            .Should()
            .Be(campaign.Id);

        assessmentStrategy.AssessmentName
            .Should()
            .NotBeNullOrWhiteSpace();

        assessmentStrategy.Content
            .Should()
            .NotBeNull();

        assessmentStrategy.StructuredContent
            .Should()
            .NotBeNull();

        assessmentStrategy.ProviderName
            .Should()
            .Be("Fake");

        assessmentStrategy.ModelName
            .Should()
            .Be("IntegrationTest");

        assessmentStrategy.Prompt
            .Should()
            .NotBeNullOrWhiteSpace();

        assessmentStrategy.RawResponse
            .Should()
            .NotBeNullOrWhiteSpace();

        assessmentStrategy.Status
            .Should()
            .Be(AssessmentStrategyStatus.Generated);

        var rounds = await db
            .Set<AssessmentRound>()
            .Where(x =>
                x.AssessmentStrategyId == assessmentStrategy.Id)
            .ToListAsync();

        rounds.Should().NotBeEmpty();

        rounds.Should()
            .AllSatisfy(round =>
            {
                round.AssessmentStrategyId
                    .Should()
                    .Be(assessmentStrategy.Id);

                round.DurationInMinutes
                    .Should()
                    .BeGreaterThan(0);

                round.TargetQuestionCount
                    .Should()
                    .BeGreaterThan(0);

                round.Competencies
                    .Should()
                    .NotBeNull();
            });
    }
}