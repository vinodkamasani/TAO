using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Application.Auth.RegisterOrganization;

namespace TAO.Api.Endpoints.Auth.RegisterOrganization;

public static class RegisterOrganizationEndpoint
{
    public static IEndpointRouteBuilder MapRegisterOrganizationEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/register-organization",
                HandleAsync)
            .AllowAnonymous()
            .WithName("RegisterOrganization")
            .WithSummary(
                "Registers a new organization and its administrator.")
            .WithDescription(
                "Creates a new organization and its initial administrator.")
            .Produces<RegisterOrganizationResponse>(
                StatusCodes.Status201Created)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status409Conflict);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterOrganizationRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RegisterOrganizationCommand(
            request.OrganizationName,
            request.OrganizationCode,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Code is
                "Organization.CodeAlreadyExists" or
                "User.EmailAlreadyExists")
            {
                return Results.Conflict(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Registration conflict",
                        Detail = result.Error.Message
                    });
            }

            return Results.BadRequest(
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Registration failed",
                    Detail = result.Error.Message
                });
        }

        return Results.Created(
            "/api/auth/me",
            result.Value);
    }
}