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
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand(
            request.Name,
            request.Code);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return result.ToApiFailure(
                httpContext,
                "Organization.CreateFailed",
                "Failed to create organization.",
                StatusCodes.Status500InternalServerError);
        }

        return result.ToCreatedApiResult(
            httpContext,
            organizationId => $"/api/organizations/{organizationId}",
            "Organization created successfully.");
    }

}