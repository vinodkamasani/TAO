using MediatR;
using TAO.Api.Extensions;
using TAO.Application.AssessmentResults.GetAssessmentSummary;

namespace TAO.Api.Endpoints.AssessmentResults;

public static class GetAssessmentSummaryEndpoint
{
    public static IEndpointRouteBuilder
        MapGetAssessmentSummaryEndpoint(
            this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/assessment-results/{assessmentSessionId:guid}",
                HandleAsync)
            .WithName("GetAssessmentSummary")
            .WithSummary("Gets the assessment result summary.")
            .WithDescription(
                "Returns the overall assessment score, competency scores, " +
                "and assessment round summaries.")
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
            new GetAssessmentSummaryQuery(
                assessmentSessionId),
            cancellationToken);

       return result.ToApiResult(httpContext, "Assessment summary retrieved successfully.");
    }
}