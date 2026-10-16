using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Application.HiringStrategies.UpdateThreshold;
using TAO.Api.Extensions;

namespace TAO.Api.Endpoints.HiringStrategies.UpdateThreshold;

public static class UpdateHiringStrategyThresholdEndpoint
{
    public static RouteGroupBuilder MapUpdateThresholdEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPut(
                "/api/campaigns/{campaignId:guid}/hiring-strategy/threshold",
                HandleAsync)
            .RequireAuthorization()
            .WithName("UpdateHiringStrategyThreshold")
            .WithTags("Hiring Strategy");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        [FromBody] UpdateHiringStrategyThresholdRequest request,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var command = new UpdateHiringStrategyThresholdCommand(
            campaignId,
            request.ThresholdPercentage);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return result.ToApiResult(httpContext);
        }

        return result.ToApiResult(httpContext,"Resume match threshold updated successfully.");
    }
}