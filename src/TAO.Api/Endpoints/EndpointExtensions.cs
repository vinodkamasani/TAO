using TAO.Api.Endpoints.Organizations;
using TAO.Api.Endpoints.Campaigns;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapOrganizationEndpoints();

        app.MapCampaignEndpoints();

        return app;
    }
}