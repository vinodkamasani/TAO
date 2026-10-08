using MediatR;
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
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAssessmentQuestionQuery(
                assessmentSessionId,
                questionId),
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