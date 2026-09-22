using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.AssessmentQuestions.Skip;

namespace TAO.Api.Endpoints.AssessmentQuestions.Skip;

public static class SkipAssessmentQuestionEndpoint
{
    public static IEndpointRouteBuilder MapSkipAssessmentQuestionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-questions/{assessmentQuestionId:guid}/skip",
                HandleAsync)
            .WithName("SkipAssessmentQuestion")
            .WithSummary("Skips an assessment question.");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentQuestionId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new SkipAssessmentQuestionCommand(
                assessmentQuestionId),
            cancellationToken);

        if (result.IsFailure)
        {
            // Use the error's ToString() to include something useful; adjust status code as appropriate.
            return Results.Problem(detail: result.Error?.ToString(), statusCode: 400);
        }

        return result.ToNoContentResult();
    }
}