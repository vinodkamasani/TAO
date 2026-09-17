using MediatR;
using TAO.Api.Extensions;
using TAO.Application.ResumeScreenings.ScreenCampaign;

namespace TAO.Api.Endpoints.Campaigns.ScreenResumes;

public static class ScreenCampaignResumesEndpoint
{
    public static RouteGroupBuilder MapScreenCampaignResumesEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
                "/{campaignId:guid}/resume-screening",
                HandleAsync)
            .WithName("ScreenCampaignResumes")
            .WithSummary("Screens all unscreened candidates in a campaign.")
            .WithDescription(
                "Screens candidates that have not already been screened for the campaign.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command =
            new ScreenCampaignResumesCommand(campaignId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToOkResult();
    }
}