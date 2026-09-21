using MediatR;
using TAO.Application.Auth.Logout;

namespace TAO.Api.Endpoints.Auth.Logout;

public static class LogoutEndpoint
{
    public static IEndpointRouteBuilder MapLogoutEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/logout",
                HandleAsync)
            .RequireAuthorization()
            .WithName("Logout")
            .WithSummary("Logs out the current user.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new LogoutCommand(),
            cancellationToken);

        return Results.NoContent();
    }
}