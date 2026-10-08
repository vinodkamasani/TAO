using MediatR;
using TAO.Application.AssessmentResults.GetAssessmentRound;

namespace TAO.Api.Endpoints.AssessmentResults.GetAssessmentRound;

public static class GetAssessmentRoundEndpoint
{
    public static IEndpointRouteBuilder MapGetAssessmentRoundEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/assessment-results/{assessmentSessionId:guid}/rounds/{roundId:guid}",
                HandleAsync)
            .WithName("GetAssessmentRound")
            .WithSummary("Gets assessment round details.")
            .WithDescription(
                "Returns the round evaluation and question-level evaluation summaries " +
                "for a recruiter or hiring manager in the same organization.")
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentSessionId,
        Guid roundId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAssessmentRoundQuery(
                assessmentSessionId,
                roundId),
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