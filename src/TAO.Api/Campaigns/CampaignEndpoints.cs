using TAO.Api.Endpoints.Campaigns.Create;

namespace TAO.Api.Endpoints.Campaigns;

public static class CampaignEndpoints
{
    public static RouteGroupBuilder MapCampaignEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/campaigns")
            .WithTags("Campaigns");

        group.MapCreateCampaignEndpoint();

        return group;
    }
}