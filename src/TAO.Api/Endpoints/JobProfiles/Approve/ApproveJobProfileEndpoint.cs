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
           Guid jobProfileId,
           [FromBody] ApproveJobProfileRequest request,
           ISender sender,
           CancellationToken cancellationToken)
    {
        var command = new ApproveJobProfileCommand(
            jobProfileId,
            request.ApprovedByUserId);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            // Use the error's ToString() to include something useful; adjust status code as appropriate.
            return Results.Problem(detail: result.Error?.ToString(), statusCode: 400);
        }

        return result.ToNoContentResult();
    }
}