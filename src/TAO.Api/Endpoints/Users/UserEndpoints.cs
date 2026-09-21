using TAO.Api.Endpoints.Users.Create;
using TAO.Api.Endpoints.Users.List;

namespace TAO.Api.Endpoints.Users;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapCreateUserEndpoint();
        group.MapGetUsersEndpoint();

        return group;
    }
}