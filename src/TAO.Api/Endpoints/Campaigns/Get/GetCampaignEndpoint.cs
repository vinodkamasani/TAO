using MediatR;
using TAO.Api.Extensions;
using TAO.Application.Campaigns.Get;

namespace TAO.Api.Endpoints.Campaigns.Get;

public static class GetCampaignEndpoint
{
    public static RouteGroupBuilder MapGetCampaignEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/{campaignId:guid}",
                HandleAsync)
            .WithName("GetCampaign")
            .WithSummary("Gets a campaign by Id.")
            .WithDescription("Returns the campaign details for the specified identifier.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignQuery(campaignId);

        var result = await sender.Send(
            query,
            cancellationToken);

        return result.ToOkResult();
    }
}
