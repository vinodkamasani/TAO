using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.Auth.Contracts;
using TAO.Application.Auth.Login;

namespace TAO.Api.Endpoints.Auth.Login;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/login",
                HandleAsync)
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Authenticates a user.")
            .WithDescription(
                "Authenticates a TAO user and creates an authentication session.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result.Value);
    }
}