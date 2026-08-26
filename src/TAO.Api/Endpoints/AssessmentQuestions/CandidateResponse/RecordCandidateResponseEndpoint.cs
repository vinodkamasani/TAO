using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.AssessmentQuestions.CandidateResponse;

namespace TAO.Api.Endpoints.AssessmentQuestions.CandidateResponse;
public static class RecordCandidateResponseEndpoint
{
    public static IEndpointRouteBuilder MapRecordCandidateResponseEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-questions/{assessmentQuestionId:guid}/response",
                HandleAsync)
            .WithName("RecordCandidateResponse")
            .WithSummary("Records the candidate's response to an assessment question.");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentQuestionId,
        [FromBody] RecordCandidateResponseRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RecordCandidateResponseCommand(
            assessmentQuestionId,
            request.Response);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToNoContentResult();
    }
}