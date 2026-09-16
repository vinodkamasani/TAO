using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.JobProfiles.Regenerate;

namespace TAO.Api.Endpoints.JobProfiles.Regenerate;

public static class RegenerateJobProfileEndpoint
{
    public static RouteGroupBuilder MapRegenerateJobProfileEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPut(
                "/{jobProfileId:guid}/regenerate",
                HandleAsync)
            .WithName("RegenerateJobProfile")
            .WithSummary("Regenerates a Job Profile using AI.")
            .WithDescription(
                "Regenerates an existing Job Profile using the recruiter-edited Job Description.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid jobProfileId,
        [FromBody] RegenerateJobProfileRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RegenerateJobProfileCommand(
            jobProfileId,
            request.OriginalJobDescription);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToNoContentResult();
    }
}