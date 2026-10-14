using MediatR;
using TAO.Api.Extensions;
using TAO.Application.AssessmentResults.GetAssessmentQuestionCode;

namespace TAO.Api.Endpoints.AssessmentResults.GetAssessmentQuestionCode;

public static class GetAssessmentQuestionCodeEndpoint
{
    public static IEndpointRouteBuilder
        MapGetAssessmentQuestionCodeEndpoint(
            this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/assessment-results/{assessmentSessionId:guid}/questions/{questionId:guid}/code",
                HandleAsync)
            .WithName("GetAssessmentQuestionCode")
            .WithSummary("Gets the candidate code for an assessment question.")
            .WithDescription(
                "Returns the candidate submitted code for an assessment question " +
                "for a recruiter or hiring manager in the same organization.")
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentSessionId,
        Guid questionId,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAssessmentQuestionCodeQuery(
                assessmentSessionId,
                questionId),
            cancellationToken);

        return result.ToApiResult(httpContext, "Candidate code retrieved successfully.");
    }
}