using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Approve;

internal sealed class ApproveHiringStrategyCommandHandler
    : IRequestHandler<ApproveHiringStrategyCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public ApproveHiringStrategyCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        ApproveHiringStrategyCommand request,
        CancellationToken cancellationToken)
    {
        var hiringStrategy = await _context
            .Set<HiringStrategy>()
            .SingleOrDefaultAsync(
                x => x.Id == request.HiringStrategyId,
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