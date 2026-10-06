using MediatR;
using TAO.Application.AssessmentSessions.Advance;

namespace TAO.Api.Endpoints.AssessmentSessions.Advance;

public static class AdvanceAssessmentSessionEndpoint
{
    public static IEndpointRouteBuilder MapAdvanceAssessmentSessionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-sessions/{assessmentSessionId:guid}/advance",
                HandleAsync)
            .WithName("AdvanceAssessmentSession")
            .WithSummary("Advances an assessment session.")
            .WithDescription(
                "Advances the assessment state machine to the next question, " +
                "next round, or assessment completion.")
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentSessionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AdvanceAssessmentSessionCommand(
                assessmentSessionId),
            cancellationToken);

        if (result.IsFailure)
        {
            return Results.Problem(
                detail: result.Error?.ToString(),
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Results.Ok(result.Value);
    }
}