using TAO.Api.Endpoints.Campaigns.Create;
using TAO.Api.Endpoints.JobProfiles.Create;
using TAO.Api.Endpoints.JobProfiles.Get;

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
        group.MapCreateJobProfileEndpoint();
        return group;
    }
}