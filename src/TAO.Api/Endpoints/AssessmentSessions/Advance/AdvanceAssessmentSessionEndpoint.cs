using MediatR;
using TAO.Api.Extensions;
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
     HttpContext httpContext,
     CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AdvanceAssessmentSessionCommand(assessmentSessionId),
            cancellationToken);

        return result.ToApiResult(
            httpContext,
            "Assessment session advanced successfully.");
    }
}