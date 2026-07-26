using TAO.Api.Endpoints.Campaigns;


namespace TAO.Api;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapOrganizationEndpoints();

        app.MapCampaignEndpoints();
        app.MapJobProfileEndpoints();
        return app;
    }
}