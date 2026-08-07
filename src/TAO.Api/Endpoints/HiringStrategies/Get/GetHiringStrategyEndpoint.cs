using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.HiringStrategies.Get;

namespace TAO.Api.Endpoints.HiringStrategies.Get;

public static class GetHiringStrategyEndpoint
{
    public static RouteGroupBuilder MapGetHiringStrategyEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/{campaignId:guid}/hiring-strategy",
                HandleAsync)
            .WithName("GetHiringStrategy")
            .WithSummary("Gets the Hiring Strategy for a campaign.")
            .WithDescription(
                "Returns the generated Hiring Strategy for the specified campaign.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetHiringStrategyQuery(campaignId);

        var result = await sender.Send(
            query,
            cancellationToken);

        return result.ToOkResult();
    }
}