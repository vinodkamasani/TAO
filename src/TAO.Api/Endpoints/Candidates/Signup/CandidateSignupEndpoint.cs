using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Application.Candidates.Signup;
using TAO.Application.Common.Interfaces;

namespace TAO.Api.Endpoints.Candidates.Signup;

public static class CandidateSignupEndpoint
{
    public static IEndpointRouteBuilder MapCandidateSignupEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/candidate/invitations/{invitationId:guid}/signup",
                HandleAsync)
            .WithName("CandidateSignup")
            .WithSummary("Creates a candidate account from an invitation.")
            .WithDescription(
                "Creates an Identity account for the invited candidate, " +
                "links it to the candidate application, accepts the invitation, " +
                "and signs the candidate in.")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        Guid invitationId,
        CandidateSignupRequest request,
        [FromServices] ISender sender,
        [FromServices] IAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        var command = new CandidateSignupCommand(
            invitationId,
            request.Email,
            request.Password);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return Results.Problem(
                detail: result.Error?.ToString(),
                statusCode: StatusCodes.Status400BadRequest);
        }

        var signInSucceeded =
            await authenticationService.SignInAsync(
                request.Email.Trim().ToLowerInvariant(),
                request.Password,
                cancellationToken);

        if (!signInSucceeded)
        {
            return Results.Problem(
                detail:
                    "The candidate account was created, " +
                    "but the candidate could not be signed in.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return Results.NoContent();
    }
}