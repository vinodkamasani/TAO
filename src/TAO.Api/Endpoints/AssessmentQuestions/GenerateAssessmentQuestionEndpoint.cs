using MediatR;
using TAO.Api.Extensions;
using TAO.Application.AssessmentQuestions.Generate;

namespace TAO.Api.Endpoints.AssessmentQuestions;

public static class GenerateAssessmentQuestionEndpoint
{
    public static IEndpointRouteBuilder MapGenerateAssessmentQuestionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-sessions/{assessmentSessionId:guid}/current-question",
                async (
                    Guid assessmentSessionId,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new GenerateAssessmentQuestionCommand(
                            assessmentSessionId),
                        cancellationToken);

                    return result.ToCreatedResult(
                        $"/api/assessment-questions/{result.Value.QuestionId}");
                })
            .WithName("GenerateAssessmentQuestion")
            .WithTags("Assessment Questions");

        return app;
    }
}