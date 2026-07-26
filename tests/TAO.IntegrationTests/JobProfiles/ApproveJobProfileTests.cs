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

namespace TAO.IntegrationTests.JobProfiles.Approve;

public sealed class ApproveJobProfileTests : IntegrationTestBase
{
    public ApproveJobProfileTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_Approve_JobProfile_When_Request_Is_Valid()
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

        var jobProfile = await TestDataFactory.CreateGeneratedJobProfileAsync(
            context,
            organization.Id,
            campaign.Id);

        var request = new
        {
            ApprovedByUserId = recruiter.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/jobprofiles/{jobProfile.Id}/approve",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var approvedJobProfile = await context
                         .Set<JobProfile>()
                         .AsNoTracking()
                         .SingleAsync(j => j.Id == jobProfile.Id);

        approvedJobProfile.Status
            .Should()
            .Be(JobProfileStatus.Approved);

        approvedJobProfile.ApprovedByUserId
            .Should()
            .Be(recruiter.Id);

        approvedJobProfile.ApprovedOn
            .Should()
            .NotBeNull();
    }
}