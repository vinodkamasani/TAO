using MediatR;
using TAO.Api.Extensions;
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
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAssessmentRoundQuery(
                assessmentSessionId,
                roundId),
            cancellationToken);

      return result.ToApiResult(httpContext, "Assessment round details retrieved successfully.");
    }
}