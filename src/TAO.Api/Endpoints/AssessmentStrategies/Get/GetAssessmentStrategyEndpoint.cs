using MediatR;
using TAO.Api.Extensions;
using TAO.Application.AssessmentStrategies.Get;

namespace TAO.Api.Endpoints.AssessmentStrategies.Get;

public static class GetAssessmentStrategyEndpoint
{
    public static RouteGroupBuilder MapGetAssessmentStrategyEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/{campaignId:guid}/assessment-strategy",
                HandleAsync)
            .WithName("GetAssessmentStrategy")
            .WithSummary("Gets the assessment strategy for a campaign.")
            .WithDescription(
                "Returns the assessment strategy associated with the specified campaign.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAssessmentStrategyQuery(
            campaignId);

        var result = await sender.Send(
            query,
            cancellationToken);

        if (result.IsFailure)
        {
            return Results.Problem(
                result.Error?.Message ?? String.Empty);
        }

        return result.ToOkResult();
    }
}