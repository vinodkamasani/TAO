using MediatR;
using TAO.Api.Extensions;
using TAO.Application.ResumeScreenings.Create;

namespace TAO.Api.Endpoints.ResumeScreenings.Create;

public static class CreateResumeScreeningEndpoint
{
    public static RouteGroupBuilder MapCreateResumeScreeningEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
                "/{candidateApplicationId:guid}/screen",
                HandleAsync)
            .WithName("CreateResumeScreening")
            .WithSummary("Generates a Resume Screening.")
            .WithDescription(
                "Generates an AI-powered Resume Screening for the specified Candidate Application.");

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid candidateApplicationId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateResumeScreeningCommand(
            candidateApplicationId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToCreatedResult(
            id => $"/api/resumescreenings/{id}");
    }
}