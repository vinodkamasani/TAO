using MediatR;
using TAO.Api.Extensions;
using TAO.Application.Candidates.GetAssessmentContext;

namespace TAO.Api.Endpoints.Candidates.Assessment;

public static class GetCandidateAssessmentContextEndpoint
{
    public static IEndpointRouteBuilder
        MapGetCandidateAssessmentContextEndpoint(
            this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/candidate/invitations/{invitationId:guid}/assessment-context",
                HandleAsync)
            .WithName("GetCandidateAssessmentContext")
            .WithSummary(
                "Gets the candidate assessment context.")
            .WithDescription(
                "Returns the candidate application and approved assessment " +
                "strategy identifiers for the authenticated candidate.")
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid invitationId,
        ISender sender,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCandidateAssessmentContextQuery(
                invitationId),
            cancellationToken);

        return result.ToApiResult(httpContext,
              "Candidate assessment context retrieved successfully.");
    }
}

  