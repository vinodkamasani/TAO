using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.AssessmentQuestions.CodeResponse;

namespace TAO.Api.Endpoints.AssessmentQuestions.CodeResponse;

public static class RecordCodeResponseEndpoint
{
    public static IEndpointRouteBuilder MapRecordCodeResponseEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-questions/{assessmentQuestionId:guid}/code-response",
                HandleAsync)
            .WithName("RecordCodeResponse")
            .WithSummary("Records the candidate's code and generates the next follow-up question.");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentQuestionId,
        [FromBody] RecordCodeResponseRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RecordCodeResponseCommand(
            assessmentQuestionId,
            request.Code);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return result.ToNoContentResult();
        }

        return Results.Ok(result.Value);
    }
}