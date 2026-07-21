using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TAO.IntegrationTests.Common;

namespace TAO.Api.IntegrationTests.Organizations;

public sealed class CreateOrganizationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateOrganizationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_Organization()
    {
        // Arrange
        var request = new
        {
            Name = "OPENAI12345",
            Code = "OPENAI12345"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/organizations",
            request);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var id = await response.Content
            .ReadFromJsonAsync<Guid>();

        id.Should().NotBeEmpty();
    }
}