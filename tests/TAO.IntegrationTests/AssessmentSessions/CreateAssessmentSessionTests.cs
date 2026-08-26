using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TAO.Api.Endpoints.AssessmentSessions.Create;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Infrastructure;
using TAO.Infrastructure.Persistence;
using TAO.IntegrationTests.Common;
using TAO.IntegrationTests.Common.TestData;

namespace TAO.IntegrationTests.AssessmentSessions;

public sealed class CreateAssessmentSessionTests
    : IntegrationTestBase
{
    public CreateAssessmentSessionTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Create_AssessmentSession_When_Request_Is_Valid()
    {
        // Arrange

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaoDbContext>();

        var organization =
            await TestDataFactory.CreateOrganizationAsync(db);

        var recruiter =
            await TestDataFactory.CreateUserAsync(
                db,
                organization.Id);

        var hiringManager =
            await TestDataFactory.CreateUserAsync(
                db,
                organization.Id,
                firstName: "Sarah",
                lastName: "Manager",
                role: UserRole.HiringManager);

        var campaign =
            await TestDataFactory.CreateCampaignAsync(
                db,
                organization.Id,
                recruiter.Id,
                hiringManager.Id);

        var candidateApplication =
            new CandidateApplication(
                organization.Id,
                campaign.Id,
                "Alex Morgan",
                "alex.morgan@example.com",
                "1234567890",
                null,
                "Contoso",
                "Hyderabad");

        db.Set<CandidateApplication>()
            .Add(candidateApplication);

        await db.SaveChangesAsync();

        var assessmentStrategy =
            await TestDataFactory.CreateAssessmentStrategyAsync(
                db,
                organization.Id,
                campaign.Id);

        assessmentStrategy.Approve(
            recruiter.Id);

        await db.SaveChangesAsync();

        // Act

        var request = new CreateAssessmentSessionRequest(
            candidateApplication.Id,
            assessmentStrategy.Id);

        var response = await Client.PostAsJsonAsync(
            "/api/assessment-sessions",
            request);

        // Assert

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var session =
            await db.Set<AssessmentSession>()
                .SingleAsync(
                    x => x.CandidateApplicationId ==
                         candidateApplication.Id
                         && x.AssessmentStrategyId ==
                         assessmentStrategy.Id);

        session.Should().NotBeNull();

        session.CandidateApplicationId
            .Should()
            .Be(candidateApplication.Id);

        session.AssessmentStrategyId
            .Should()
            .Be(assessmentStrategy.Id);

        session.Status
            .Should()
            .Be(AssessmentSessionStatus.NotStarted);

        session.ConsentVersion
            .Should()
            .Be(1);

        session.ConsentAcceptedOn
            .Should()
            .NotBeNull();

        session.AssessmentExpiresOn
            .Should()
            .BeAfter(DateTime.UtcNow.AddHours(23));

        session.AssessmentExpiresOn
            .Should()
            .BeBefore(DateTime.UtcNow.AddHours(25));

        session.StrategySnapshot
            .Should()
            .NotBeNull();

        var sessionRounds =
            await db.Set<AssessmentSessionRound>()
                .Where(x =>
                    x.AssessmentSessionId == session.Id)
                .OrderBy(x => x.Order)
                .ToListAsync();

        sessionRounds
            .Should()
            .HaveCount(1);

        var sessionRound = sessionRounds[0];

        sessionRound.AssessmentRoundId
            .Should()
            .NotBeEmpty();

        sessionRound.Order
            .Should()
            .Be(1);

        sessionRound.Type
            .Should()
            .Be(AssessmentRoundType.Coding);

        sessionRound.Difficulty
            .Should()
            .Be(AssessmentDifficulty.Medium);

        sessionRound.DurationInMinutes
            .Should()
            .Be(60);

        sessionRound.TargetQuestionCount
            .Should()
            .Be(3);

        sessionRound.Status
            .Should()
            .Be(AssessmentSessionRoundStatus.NotStarted);

        sessionRound.Competencies
            .Should()
            .HaveCount(2);

        var cSharp =
            sessionRound.Competencies
                .Single(x => x.Name == "C#");

        cSharp.Priority
            .Should()
            .Be("High");

        cSharp.MinimumPassPercentage
            .Should()
            .Be(70);

        var aspNetCore =
            sessionRound.Competencies
                .Single(x => x.Name == "ASP.NET Core");

        aspNetCore.Priority
            .Should()
            .Be("High");

        aspNetCore.MinimumPassPercentage
            .Should()
            .Be(70);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_CandidateApplication_DoesNotExist()
    {
        // Arrange

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaoDbContext>();

        var organization =
            await TestDataFactory.CreateOrganizationAsync(db);

        var recruiter =
            await TestDataFactory.CreateUserAsync(
                db,
                organization.Id);

        var hiringManager =
            await TestDataFactory.CreateUserAsync(
                db,
                organization.Id,
                firstName: "Sarah",
                lastName: "Manager",
                role: UserRole.HiringManager);

        var campaign =
            await TestDataFactory.CreateCampaignAsync(
                db,
                organization.Id,
                recruiter.Id,
                hiringManager.Id);

        var assessmentStrategy =
            await TestDataFactory.CreateAssessmentStrategyAsync(
                db,
                organization.Id,
                campaign.Id);

        assessmentStrategy.Approve(
            recruiter.Id);

        await db.SaveChangesAsync();

        var request = new CreateAssessmentSessionRequest(
            Guid.CreateVersion7(),
            assessmentStrategy.Id);

        // Act

        var response = await Client.PostAsJsonAsync(
            "/api/assessment-sessions",
            request);

        // Assert

        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);

        var sessionCount =
            await db.Set<AssessmentSession>()
                .CountAsync();

        sessionCount.Should()
            .Be(0);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_AssessmentStrategy_DoesNotExist()
    {
        // Arrange

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaoDbContext>();

        var organization =
            await TestDataFactory.CreateOrganizationAsync(db);

        var recruiter =
            await TestDataFactory.CreateUserAsync(
                db,
                organization.Id);

        var hiringManager =
            await TestDataFactory.CreateUserAsync(
                db,
                organization.Id,
                firstName: "Sarah",
                lastName: "Manager",
                role: UserRole.HiringManager);

        var campaign =
            await TestDataFactory.CreateCampaignAsync(
                db,
                organization.Id,
                recruiter.Id,
                hiringManager.Id);

        var candidateApplication =
            new CandidateApplication(
                organization.Id,
                campaign.Id,
                "Alex Morgan",
                "alex.morgan@example.com",
                "1234567890",
                null,
                "Contoso",
                "Hyderabad");

        db.Set<CandidateApplication>()
            .Add(candidateApplication);

        await db.SaveChangesAsync();

        var request = new CreateAssessmentSessionRequest(
            candidateApplication.Id,
            Guid.CreateVersion7());

        // Act

        var response = await Client.PostAsJsonAsync(
            "/api/assessment-sessions",
            request);

        // Assert

        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);

        var sessionCount =
            await db.Set<AssessmentSession>()
                .CountAsync();

        sessionCount.Should()
            .Be(0);
    }

    [Fact]
    public async Task Should_Return_BadRequest_When_AssessmentStrategy_Is_Not_Approved()
    {
        // Arrange

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaoDbContext>();

        var organization =
            await TestDataFactory.CreateOrganizationAsync(db);

        var recruiter =
            await TestDataFactory.CreateUserAsync(
                db,
                organization.Id);

        var hiringManager =
            await TestDataFactory.CreateUserAsync(
                db,
                organization.Id,
                firstName: "Sarah",
                lastName: "Manager",
                role: UserRole.HiringManager);

        var campaign =
            await TestDataFactory.CreateCampaignAsync(
                db,
                organization.Id,
                recruiter.Id,
                hiringManager.Id);

        var candidateApplication =
            new CandidateApplication(
                organization.Id,
                campaign.Id,
                "Alex Morgan",
                "alex.morgan@example.com",
                "1234567890",
                null,
                "Contoso",
                "Hyderabad");

        db.Set<CandidateApplication>()
            .Add(candidateApplication);

        await db.SaveChangesAsync();

        var assessmentStrategy =
            await TestDataFactory.CreateAssessmentStrategyAsync(
                db,
                organization.Id,
                campaign.Id);

        // Intentionally not approving the strategy.

        // Act

        var request = new CreateAssessmentSessionRequest(
            candidateApplication.Id,
            assessmentStrategy.Id);

        var response = await Client.PostAsJsonAsync(
            "/api/assessment-sessions",
            request);

        // Assert

        response.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);

        var sessionCount =
            await db.Set<AssessmentSession>()
                .CountAsync();

        sessionCount.Should()
            .Be(0);
    }
}