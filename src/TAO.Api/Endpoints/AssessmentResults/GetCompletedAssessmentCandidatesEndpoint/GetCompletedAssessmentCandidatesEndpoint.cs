using MediatR;
using TAO.Application.AssessmentResults.GetCompletedAssessmentCandidates;

namespace TAO.Api.Endpoints.AssessmentResults.GetCompletedAssessmentCandidatesEndpoint;

public static class GetCompletedAssessmentCandidatesEndpoint
{
    public static IEndpointRouteBuilder
        MapGetCompletedAssessmentCandidatesEndpoint(
            this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/assessment-results/campaigns/{campaignId:guid}/candidates",
                HandleAsync)
            .WithName("GetCompletedAssessmentCandidates")
            .WithSummary(
                "Gets candidates who completed the assessment.")
            .WithDescription(
                "Returns candidates who completed an assessment for the specified campaign " +
                "along with their assessment session identifiers.")
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCompletedAssessmentCandidatesQuery(
                campaignId),
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
