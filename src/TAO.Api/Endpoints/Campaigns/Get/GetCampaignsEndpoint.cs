using MediatR;
using TAO.Api.Extensions;
using TAO.Application.Campaigns.Get;

namespace TAO.Api.Endpoints.Campaigns.Get;

public static class GetCampaignsEndpoint
{
    public static RouteGroupBuilder MapGetCampaignsEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                string.Empty,
                HandleAsync)
            .WithName("GetCampaigns")
            .WithSummary("Gets all campaigns.")
            .WithDescription("Returns a list of all campaigns ordered by most recently created first.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignsQuery();

        var result = await sender.Send(
            query,
            cancellationToken);

        if (result.IsFailure)
        {
            return Results.Problem(
                result.Error?.Message ?? String.Empty);
        }


        return result.ToOkResult();
    }
}
