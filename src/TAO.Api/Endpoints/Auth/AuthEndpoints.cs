using TAO.Api.Endpoints.Auth.Login;
using TAO.Api.Endpoints.Auth.Logout;
using TAO.Api.Endpoints.Auth.Me;
using TAO.Api.Endpoints.Auth.RegisterOrganization;

namespace TAO.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapRegisterOrganizationEndpoint();
        group.MapLoginEndpoint();

        group.MapMeEndpoint();
        group.MapLogoutEndpoint();

        return group;
    }
}