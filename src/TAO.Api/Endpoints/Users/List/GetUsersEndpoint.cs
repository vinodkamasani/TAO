using MediatR;
using TAO.Api.Extensions;
using TAO.Application.Users.List;

namespace TAO.Api.Endpoints.Users.List;

public static class GetUsersEndpoint
{
    public static IEndpointRouteBuilder MapGetUsersEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/",
                HandleAsync)
            .WithName("GetUsers")
            .WithSummary("Gets users in the current organization.")
            .WithDescription(
                "Returns all users belonging to the current user's organization.")
            .Produces<IReadOnlyList<UserListItemDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetUsersQuery(),
            cancellationToken);


        if (result.IsFailure)
        {
            // Use the error's ToString() to include something useful; adjust status code as appropriate.
            return Results.Problem(detail: result.Error?.ToString(), statusCode: 400);
        }
        return result.ToOkResult();
    }
}