using MediatR;
using TAO.Api.Extensions;
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
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCurrentUserQuery(),
            cancellationToken);

        return result.ToApiResult(httpContext, "Current user retrieved successfully.");
    }
}