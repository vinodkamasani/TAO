using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.Organizations.Create;

namespace TAO.Api.Endpoints.Organizations.Create;

public static class CreateOrganizationEndpoint
{
    public static IEndpointRouteBuilder MapCreateOrganizationEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/",
                HandleAsync)
            .WithName("CreateOrganization")
            .WithSummary("Creates a new organization.")
            .WithDescription("Creates a new organization.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateOrganizationRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand(
            request.Name,
            request.Code);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            // Use the error's ToString() to include something useful; adjust status code as appropriate.
            return Results.Problem(detail: result.Error?.ToString(), statusCode: 400);
        }

        return result.ToCreatedResult(
      $"/api/organizations/{result.Value}");
    }
}