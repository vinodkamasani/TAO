using MediatR;
using TAO.Api.Extensions;
using TAO.Application.CandidateApplications.SendRecommendedEmails;

namespace TAO.Api.Endpoints.CandidateApplications.SendRecommendedEmails;

public static class SendRecommendedEmailsEndpoint
{
    public static RouteGroupBuilder MapSendRecommendedEmailsEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
                "/{campaignId:guid}/send-recommended-emails",
                HandleAsync)
            .WithName("SendRecommendedCandidateEmails")
            .WithSummary("Sends emails to recommended candidates.")
            .WithDescription(
                "Sends an email to all candidates recommended for the campaign.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid campaignId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SendRecommendedEmailsCommand(campaignId);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            // Use the error's ToString() to include something useful; adjust status code as appropriate.
            return Results.Problem(detail: result.Error?.ToString(), statusCode: 400);
        }

        return result.ToOkResult();
    }
}