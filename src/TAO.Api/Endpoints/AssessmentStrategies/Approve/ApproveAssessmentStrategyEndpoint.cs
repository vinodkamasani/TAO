using MediatR;
using TAO.Api.Extensions;
using TAO.Application.AssessmentStrategies.Approve;

namespace TAO.Api.Endpoints.AssessmentStrategies.Approve;

public static class ApproveAssessmentStrategyEndpoint
{
    public static RouteGroupBuilder MapApproveAssessmentStrategyEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
            "/assessment-strategies/{assessmentStrategyId:guid}/approve",
            HandleAsync)
            .WithName("ApproveAssessmentStrategy")
            .WithSummary("Approves an assessment strategy.")
            .WithDescription(
                "Approves a generated assessment strategy so it can be used by TAO Assess.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> HandleAsync(
        Guid assessmentStrategyId,
        ApproveAssessmentStrategyRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ApproveAssessmentStrategyCommand(
            assessmentStrategyId,
            request.ApprovedByUserId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToNoContentResult();
    }
}