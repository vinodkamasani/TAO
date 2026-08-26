using MediatR;
using TAO.Api.Extensions;
using TAO.Application.AssessmentStrategies.Create;

namespace TAO.Api.Endpoints.AssessmentStrategies.Create;

public static class CreateAssessmentStrategyEndpoint
{
    public static RouteGroupBuilder MapCreateAssessmentStrategyEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
            "/{campaignId:guid}/assessment-strategy",
            HandleAsync)
            .WithName("CreateAssessmentStrategy")
            .WithSummary("Generates an assessment strategy for a campaign.")
            .WithDescription(
                "Generates an AI-suggested assessment strategy using the approved Job Profile and Hiring Strategy.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateAssessmentStrategyCommand(
            campaignId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToCreatedResult(
            $"/api/campaigns/{campaignId}/assessment-strategy");
    }
}