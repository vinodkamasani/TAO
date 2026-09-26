using TAO.Api.Endpoints.AssessmentSessions.Create;
using TAO.Api.Endpoints.AssessmentSessions.Get;

namespace TAO.Api.Endpoints.AssessmentSessions;

public static class AssessmentSessionEndpoints
{
    public static RouteGroupBuilder MapAssessmentSessionEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/assessment-sessions")
            .WithTags("Assessment Sessions");

        group.MapCreateAssessmentSessionEndpoint();
        group.MapGetAssessmentSessionEndpoint();

        return group;
    }
}