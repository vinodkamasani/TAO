using MediatR;
using TAO.Api.Extensions;
using TAO.Application.JobProfiles.Get;

namespace TAO.Api.Endpoints.JobProfiles.Get;

public static class GetJobProfileByCampaignEndpoint
{
    public static RouteGroupBuilder MapGetJobProfileByCampaignEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/campaign/{campaignId:guid}",
                HandleAsync)
            .WithName("GetJobProfileByCampaign")
            .WithSummary("Gets a Job Profile by Campaign Id.")
            .WithDescription(
                "Returns the generated Job Profile for the specified campaign identifier.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetJobProfileByCampaignQuery(campaignId);

        var result = await sender.Send(
            query,
            cancellationToken);

        return result.ToOkResult();
    }
}