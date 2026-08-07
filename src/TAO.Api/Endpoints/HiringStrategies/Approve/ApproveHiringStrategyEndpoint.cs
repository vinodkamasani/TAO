using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.HiringStrategies.Approve;

namespace TAO.Api.Endpoints.HiringStrategies.Approve;

public static class ApproveHiringStrategyEndpoint
{
    public static RouteGroupBuilder MapApproveHiringStrategyEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
                "/{hiringStrategyId:guid}/approve",
                HandleAsync)
            .WithName("ApproveHiringStrategy")
            .WithSummary("Approves a Hiring Strategy.")
            .WithDescription(
                "Approves the generated Hiring Strategy for recruiter use.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid hiringStrategyId,
        [FromBody] ApproveHiringStrategyRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ApproveHiringStrategyCommand(
            hiringStrategyId,
            request.ApprovedByUserId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToNoContentResult();
    }
}