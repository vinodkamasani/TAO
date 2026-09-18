using MediatR;
using TAO.Api.Extensions;
using TAO.Application.CandidateApplications.Get;

namespace TAO.Api.Endpoints.CandidateApplications.Get;

public static class GetCandidateApplicationsEndpoint
{
    public static RouteGroupBuilder MapGetCandidateApplicationsEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/{campaignId:guid}/candidates",
                HandleAsync)
            .WithName("GetCandidateApplications")
            .WithSummary("Gets candidates for a campaign.")
            .WithDescription(
                "Returns all candidates associated with the specified campaign " +
                "along with their screening status.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCandidateApplicationsQuery(
            campaignId);

        var result = await sender.Send(
            query,
            cancellationToken);

        return result.ToOkResult();
    }
}