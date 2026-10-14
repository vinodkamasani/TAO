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
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var command = new CreateJobProfileCommand(
            campaignId,
            request.OriginalJobDescription);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return result.ToApiFailure(
                httpContext,
                "JobProfile.CreateFailed",
                "Failed to create job profile.",
                StatusCodes.Status500InternalServerError);
        }

        return result.ToCreatedApiResult(
            httpContext,
            jobProfile => $"/api/jobprofiles/{jobProfile.Id}",
            "Job profile created successfully.");
    }

}