using MediatR;
using TAO.Api.Extensions;
using TAO.Application.Campaigns.Get;

namespace TAO.Api.Endpoints.Campaigns.Get;

public static class GetCampaignJobProfileEndpoint
{
    public static RouteGroupBuilder MapGetCampaignJobProfileEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/{campaignId:guid}/job-profile",
                HandleAsync)
            .WithName("GetCampaignJobProfile")
            .WithSummary("Gets the Job Profile for a campaign.")
            .WithDescription(
                "Returns the generated Job Profile associated with the specified campaign ID. " +
                "Shows original job description, AI-generated content, structured profile, and approval status.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignJobProfileQuery(campaignId);

        var result = await sender.Send(
            query,
            cancellationToken);

        return result.ToOkResult();
    }
}
