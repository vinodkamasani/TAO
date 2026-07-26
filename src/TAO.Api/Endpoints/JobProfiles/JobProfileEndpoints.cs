using TAO.Api.Endpoints.JobProfiles.Approve;
using TAO.Api.Endpoints.JobProfiles.Get;

public static class JobProfileEndpoints
{
    public static RouteGroupBuilder MapJobProfileEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/jobprofiles")
            .WithTags("Job Profiles");

        group.MapGetJobProfileEndpoint();
        group.MapApproveJobProfileEndpoint();

        return group;
    }
}