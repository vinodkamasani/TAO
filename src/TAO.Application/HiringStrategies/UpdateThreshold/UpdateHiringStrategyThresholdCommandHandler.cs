using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.UpdateThreshold;

internal sealed class UpdateHiringStrategyThresholdCommandHandler
    : IRequestHandler<UpdateHiringStrategyThresholdCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateHiringStrategyThresholdCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateHiringStrategyThresholdCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate authentication.
        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "HiringStrategy.Unauthorized",
                    "The current user is not authenticated."));
        }

        // 2. Validate organization context.
        if (_currentUser.OrganizationId is null ||
            _currentUser.OrganizationId == Guid.Empty)
        {
            return Result.Failure(
                Error.Forbidden(
                    "HiringStrategy.OrganizationRequired",
                    "The current user does not belong to an organization."));
        }

        // 3. Validate threshold.
        if (request.ThresholdPercentage is < 0 or > 100)
        {
            return Result.Failure(
                Error.Validation(
                    "HiringStrategy.InvalidThreshold",
                    "Resume match threshold must be between 0 and 100."));
        }

        // 4. Load the strategy within the current organization.
        var hiringStrategy = await _context
            .Set<HiringStrategy>()
            .FirstOrDefaultAsync(
                x =>
                    x.CampaignId == request.CampaignId &&
                    x.OrganizationId == _currentUser.OrganizationId.Value,
                cancellationToken);

        if (hiringStrategy is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "HiringStrategy.NotFound",
                    "Hiring strategy was not found for this campaign."));
        }

        // 5. Update both existing content representations.
        try
        {
            hiringStrategy.UpdateResumeMatchThreshold(
                request.ThresholdPercentage);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Result.Failure(
                Error.Validation(
                    "HiringStrategy.InvalidThreshold",
                    "Resume match threshold must be between 0 and 100."));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(
                Error.Validation(
                    "HiringStrategy.InvalidContent",
                    ex.Message));
        }

        // 6. Persist the changes.
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}