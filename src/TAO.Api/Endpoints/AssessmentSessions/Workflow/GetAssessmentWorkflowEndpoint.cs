using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Application.AssessmentSessions.Workflow;

namespace TAO.Api.Endpoints.AssessmentSessions.Workflow;

public static class GetAssessmentWorkflowEndpoint
{
    public static IEndpointRouteBuilder MapGetAssessmentWorkflowEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/assessment-sessions/{assessmentSessionId:guid}/workflow",
                HandleAsync)
            .WithName("GetAssessmentWorkflow")
            .WithSummary(
                "Gets the candidate's assessment workflow progress.")
            .WithDescription(
                "Gets assessment progress including completed rounds, " +
                "completed questions, remaining questions and the current round/question.");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentSessionId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAssessmentWorkflowQuery(
            assessmentSessionId);

        var result = await sender.Send(
            query,
            cancellationToken);

        if (result.IsFailure)
        {
            return Results.Problem(
                detail: result.Error?.ToString(),
                statusCode: 400);
        }

        return Results.Ok(result.Value);
    }
}