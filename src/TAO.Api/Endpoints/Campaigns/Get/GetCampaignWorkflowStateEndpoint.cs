using MediatR;
using TAO.Api.Extensions;
using TAO.Application.Campaigns.Get;

namespace TAO.Api.Endpoints.Campaigns.Get;

public static class GetCampaignWorkflowStateEndpoint
{
    public static RouteGroupBuilder MapGetCampaignWorkflowStateEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/{campaignId:guid}/workflow-state",
                HandleAsync)
            .WithName("GetCampaignWorkflowState")
            .WithSummary("Gets the workflow state of a campaign.")
            .WithDescription(
                "Returns detailed information about the current stage of the campaign workflow, " +
                "including status of Job Profile, Hiring Strategy, Assessment Strategy, and Resume Imports. " +
                "Shows completion percentage and progress through each stage.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignWorkflowStateQuery(campaignId);

        var result = await sender.Send(
            query,
            cancellationToken);

        return result.ToOkResult();
    }
}
