using MediatR;
using TAO.Api.Extensions;
using TAO.Application.JobProfiles.Get;

namespace TAO.Api.Endpoints.JobProfiles.Get;

public static class GetJobProfileEndpoint
{
    public static RouteGroupBuilder MapGetJobProfileEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/{jobProfileId:guid}",
                HandleAsync)
            .WithName("GetJobProfile")
            .WithSummary("Gets a Job Profile by Id.")
            .WithDescription(
                "Returns the generated Job Profile for the specified identifier.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid jobProfileId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetJobProfileQuery(jobProfileId);

        var result = await sender.Send(
            query,
            cancellationToken);

        return result.ToOkResult();
    }
}