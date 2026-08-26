using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.AssessmentQuestions.Complete;

namespace TAO.Api.Endpoints.AssessmentQuestions.Complete;

public static class CompleteAssessmentQuestionEndpoint
{
    public static IEndpointRouteBuilder MapCompleteAssessmentQuestionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-questions/{assessmentQuestionId:guid}/complete",
                HandleAsync)
            .WithName("CompleteAssessmentQuestion")
            .WithSummary("Completes an assessment question.");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentQuestionId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CompleteAssessmentQuestionCommand(
                assessmentQuestionId),
            cancellationToken);

        return result.ToNoContentResult();
    }
}