using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.AssessmentSessions.Create;

namespace TAO.Api.Endpoints.AssessmentSessions.Create;

public static class CreateAssessmentSessionEndpoint
{
    public static RouteGroupBuilder MapCreateAssessmentSessionEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName("CreateAssessmentSession")
            .WithSummary("Creates an assessment session.")
            .WithDescription(
                "Creates an assessment session for a candidate using an approved assessment strategy.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateAssessmentSessionRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateAssessmentSessionCommand(
            request.CandidateApplicationId,
            request.AssessmentStrategyId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToCreatedResult(
            $"/api/assessment-sessions/{result.Value}");
    }
}