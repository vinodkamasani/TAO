using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.JobProfiles.Create;

namespace TAO.Api.Endpoints.JobProfiles.Create;

public static class CreateJobProfileEndpoint
{
    public static RouteGroupBuilder MapCreateJobProfileEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
                "/{campaignId:guid}/job-profile",
                HandleAsync)
            .WithName("CreateJobProfile")
            .WithSummary("Generates an AI Job Profile for a campaign.")
            .WithDescription(
                "Generates a Job Profile using AI based on the original job description.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        [FromBody] CreateJobProfileRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateJobProfileCommand(
            campaignId,
            request.OriginalJobDescription);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToCreatedResult(
            $"/api/jobprofiles/{result.Value}");
    }
}