using TAO.Api.Endpoints.CandidateApplications.SendRecommendedEmails;

public static class CandidateApplicationEndpoints
{
    public static RouteGroupBuilder MapCandidateApplicationEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/candidate-applications")
            .WithTags("Candidate Applications");

        group.MapSendRecommendedEmailsEndpoint();

        return group;
    }
}