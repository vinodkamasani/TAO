using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.AssessmentSessions.Start;

namespace TAO.Api.Endpoints.AssessmentSessions;

public static class StartAssessmentSessionEndpoint
{
    public static IEndpointRouteBuilder MapStartAssessmentSessionEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/assessment-sessions/{assessmentSessionId:guid}/start",
                async (
                    Guid assessmentSessionId,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new StartAssessmentSessionCommand(
                            assessmentSessionId),
                        cancellationToken);

                    return result.ToNoContentResult();
                })
            .WithName("StartAssessmentSession")
            .WithTags("Assessment Sessions");

        return app;
    }
}