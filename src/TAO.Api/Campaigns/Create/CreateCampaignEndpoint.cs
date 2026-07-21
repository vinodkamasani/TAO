using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.Campaigns.Create;

namespace TAO.Api.Endpoints.Campaigns.Create;

public static class CreateCampaignEndpoint
{
    public static RouteGroupBuilder MapCreateCampaignEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName("CreateCampaign")
            .WithSummary("Creates a new campaign.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateCampaignRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateCampaignCommand(
            request.OrganizationId,
            request.Name,
            request.ReferenceNumber,
            request.RecruiterId,
            request.HiringManagerId,
            request.NumberOfOpenings);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToCreatedResult(
            $"/api/campaigns/{result.Value}");
    }
}