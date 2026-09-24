using MediatR;
using TAO.Application.Auth.Contracts;
using TAO.Application.Auth.Me;

namespace TAO.Api.Endpoints.Auth.Me;

public static class GetCurrentUserEndpoint
{
    public static IEndpointRouteBuilder MapMeEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/me",
                HandleAsync)
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithSummary("Gets the current authenticated user.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCurrentUserQuery(),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result.Value);
    }
}