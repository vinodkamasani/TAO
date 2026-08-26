using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Application.AssessmentQuestions.FollowUp;
using TAO.Api.Extensions;

namespace TAO.Api.Endpoints.AssessmentQuestions.FollowUp;

public static class GenerateFollowUpEndpoint
{
    public static IEndpointRouteBuilder MapGenerateFollowUpEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-questions/{assessmentQuestionId:guid}/follow-up",
                HandleAsync)
            .WithName("GenerateAssessmentFollowUp")
            .WithSummary("Generates the next AI follow-up question.");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentQuestionId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GenerateFollowUpCommand(
                assessmentQuestionId),
            cancellationToken);

        if (result.IsFailure)
        {
            // Use the error's ToString() to include something useful; adjust status code as appropriate.
            return Results.Problem(detail: result.Error?.ToString(), statusCode: 400);
        }

        return Results.Ok(result.Value);
    }
}