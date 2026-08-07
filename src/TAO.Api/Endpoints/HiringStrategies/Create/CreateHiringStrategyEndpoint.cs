using MediatR;
using TAO.Api.Extensions;
using TAO.Application.HiringStrategies.Create;

namespace TAO.Api.Endpoints.HiringStrategies.Create;

public static class CreateHiringStrategyEndpoint
{
    public static RouteGroupBuilder MapCreateHiringStrategyEndpoint(
        this RouteGroupBuilder group)
    {

        group.MapPost(
                "/{campaignId:guid}/hiring-strategy",
                HandleAsync)
            .WithName("CreateHiringStrategy")
            .WithSummary("Generates an AI Hiring Strategy for a campaign.")
            .WithDescription(
                "Generates a Hiring Strategy using AI based on the approved Job Profile.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateHiringStrategyCommand(campaignId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToCreatedResult(
            $"/api/hiringstrategies/{result.Value}");
    }
}