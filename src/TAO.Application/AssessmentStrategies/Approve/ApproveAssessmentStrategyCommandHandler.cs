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

    public ApproveAssessmentStrategyCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        ApproveAssessmentStrategyCommand request,
        CancellationToken cancellationToken)
    {
        var assessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentStrategyId,
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