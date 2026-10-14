using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.AssessmentSessions.Get;

namespace TAO.Api.Endpoints.AssessmentSessions.Get;

public static class GetAssessmentSessionEndpoint
{
    public static RouteGroupBuilder MapGetAssessmentSessionEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapGet(
                "/{assessmentSessionId:guid}",
                HandleAsync)
            .WithName("GetAssessmentSession")
            .WithSummary("Gets an assessment session.")
            .WithDescription(
                "Gets an assessment session and its rounds.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentSessionId,
        [FromServices] ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var query = new GetAssessmentSessionQuery(
            assessmentSessionId);

        var result = await sender.Send(
            query,
            cancellationToken);

      return result.ToApiResult(httpContext,
          "Assessment session found.");
    }
}