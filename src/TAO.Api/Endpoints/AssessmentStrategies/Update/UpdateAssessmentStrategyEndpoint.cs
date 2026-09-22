using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Application.AssessmentStrategies.Update;

namespace TAO.Api.Endpoints.AssessmentStrategies.Update;

public static class UpdateAssessmentStrategyEndpoint
{
    public static IEndpointRouteBuilder
        MapUpdateAssessmentStrategyEndpoint(
            this IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/{assessmentStrategyId:guid}",
                HandleAsync)
            .RequireAuthorization()
            .WithName("UpdateAssessmentStrategy")
            .WithSummary(
                "Updates an Assessment Strategy.")
            .Produces<UpdateAssessmentStrategyResponse>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentStrategyId,
        [FromBody] UpdateAssessmentStrategyRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAssessmentStrategyCommand(
            assessmentStrategyId,
            request.AssessmentName,
            request.Rounds
                .Select(round =>
                    new UpdateAssessmentRoundCommand(
                        round.Order,
                        round.Type,
                        round.Difficulty,
                        round.DurationInMinutes,
                        round.QuestionCount,
                        round.Competencies
                            .Select(competency =>
                                new UpdateAssessmentCompetencyCommand(
                                    competency.Name,
                                    competency.Priority,
                                    competency.MinimumPassPercentage))
                            .ToList()))
                .ToList());

        var result = await sender.Send(
            command,
            cancellationToken);


        if (result.IsFailure)
        {
            return Results.Problem(
                result.Error?.Message ?? String.Empty);
        }
        return Results.Ok(result.Value);
    }
}