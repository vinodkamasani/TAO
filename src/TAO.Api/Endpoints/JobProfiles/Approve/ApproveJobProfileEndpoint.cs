using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.HiringStrategies.Approve;
using TAO.Application.JobProfiles.Approve;

namespace TAO.Api.Endpoints.JobProfiles.Approve;

public static class ApproveJobProfileEndpoint
{
    public static RouteGroupBuilder MapApproveJobProfileEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
                "/{jobProfileId:guid}/approve",
                HandleAsync)
            .WithName("ApproveJobProfile")
            .WithSummary("Approves a Job Profile.")
            .WithDescription(
                "Approves the specified Job Profile.");

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