using MediatR;
using TAO.Application.AssessmentResults.GetAssessmentQuestionConversation;

namespace TAO.Api.Endpoints.AssessmentResults.GetAssessmentQuestionConversation;

public static class GetAssessmentQuestionConversationEndpoint
{
    public static IEndpointRouteBuilder
        MapGetAssessmentQuestionConversationEndpoint(
            this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/assessment-results/{assessmentSessionId:guid}/questions/{questionId:guid}/conversation",
                HandleAsync)
            .WithName("GetAssessmentQuestionConversation")
            .WithSummary("Gets the conversation for an assessment question.")
            .WithDescription(
                "Returns the stored assessment conversation for a recruiter or hiring manager " +
                "in the same organization.")
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
            new GetAssessmentQuestionConversationQuery(
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