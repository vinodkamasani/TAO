using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.AssessmentQuestionEvaluations.Evaluate;

namespace TAO.Api.Endpoints.AssessmentQuestionEvaluations.Evaluate;

public static class EvaluateAssessmentQuestionEndpoint
{
    public static IEndpointRouteBuilder MapEvaluateAssessmentQuestionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-questions/{assessmentQuestionId:guid}/evaluate",
                HandleAsync)
            .WithName("EvaluateAssessmentQuestion")
            .WithSummary("Evaluates a completed assessment question.");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentQuestionId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new EvaluateAssessmentQuestionCommand(
                assessmentQuestionId),
            cancellationToken);

        return result.ToNoContentResult();
    }
}