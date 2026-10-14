using MediatR;
using TAO.Api.Extensions;
using TAO.Application.AssessmentResults.GetAssessmentQuestion;

namespace TAO.Api.Endpoints.AssessmentResults.GetAssessmentQuestion;

public static class GetAssessmentQuestionEndpoint
{
    public static IEndpointRouteBuilder MapGetAssessmentQuestionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/assessment-results/{assessmentSessionId:guid}/questions/{questionId:guid}",
                HandleAsync)
            .WithName("GetAssessmentQuestion")
            .WithSummary("Gets assessment question details.")
            .WithDescription(
                "Returns the question and its evaluation for a recruiter " +
                "or hiring manager in the same organization.")
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
            new GetAssessmentQuestionQuery(
                assessmentSessionId,
                questionId),
            cancellationToken);

        return result.ToApiResult(httpContext, "Assessment question details retrieved successfully.");
    }
}