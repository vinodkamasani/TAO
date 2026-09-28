using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Generate;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Approve;

internal sealed class ApproveHiringStrategyCommandHandler
    : IRequestHandler<ApproveHiringStrategyCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser; 

    public ApproveHiringStrategyCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        ApproveHiringStrategyCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<Result>.Failure(
                Error.Unauthorized(
                    "ApproveHiringStrategy.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<Result>.Failure(
                Error.Unauthorized(
                    "ApproveHiringStrategy.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }


        var hiringStrategy = await _context
            .Set<HiringStrategy>()
            .SingleOrDefaultAsync(
                x => x.Id == request.HiringStrategyId && x.OrganizationId == organizationId.Value,
                cancellationToken);

        if (hiringStrategy is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "HiringStrategy.NotFound",
                    "Hiring Strategy was not found."));
        }

        if (hiringStrategy.Status == HiringStrategyStatus.Approved)
        {
            return Result.Failure(
                Error.Conflict(
                    "HiringStrategy.AlreadyApproved",
                    "Hiring Strategy has already been approved."));
        }

        hiringStrategy.Approve(request.ApprovedByUserId);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}