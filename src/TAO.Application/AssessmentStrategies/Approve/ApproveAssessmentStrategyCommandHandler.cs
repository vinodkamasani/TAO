using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Approve;

internal sealed class ApproveAssessmentStrategyCommandHandler
    : IRequestHandler<ApproveAssessmentStrategyCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ApproveAssessmentStrategyCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        ApproveAssessmentStrategyCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "ApproveAssessmentStrategyCommandHandler.Unauthorized",
                    "The current user is not authenticated."));
        }
        var organizationId = _currentUser.OrganizationId;

        var assessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentStrategyId && x.OrganizationId == organizationId.Value,
                cancellationToken);

        if (assessmentStrategy is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentStrategy.NotFound",
                    $"Assessment Strategy '{request.AssessmentStrategyId}' was not found."));
        }

        assessmentStrategy.Approve(
            request.ApprovedByUserId);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}