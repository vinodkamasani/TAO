using TAO.Api.Endpoints.Organizations.Create;

public static class OrganizationEndpoints
{
    public static RouteGroupBuilder MapOrganizationEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/organizations")
            .WithTags("Organizations");

        group.MapCreateOrganizationEndpoint();
        // group.MapGetOrganizationEndpoint();
        // group.MapUpdateOrganizationEndpoint();

        return group;
    }
}