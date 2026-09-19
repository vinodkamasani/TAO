using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Get;

public sealed class GetAssessmentStrategyQueryHandler
    : IRequestHandler<
        GetAssessmentStrategyQuery,
        Result<GetAssessmentStrategyResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAssessmentStrategyQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetAssessmentStrategyResponse>> Handle(
        GetAssessmentStrategyQuery request,
        CancellationToken cancellationToken)
    {
        var assessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .AsNoTracking()
            .Where(x => x.CampaignId == request.CampaignId)
            .Select(x => new GetAssessmentStrategyResponse(
                x.Id,
                x.OrganizationId,
                x.CampaignId,
                x.AssessmentName,
                x.Content,
                x.StructuredContent,
                x.Status.ToString(),
                x.GeneratedOn,
                x.ApprovedByUserId,
                x.ApprovedOn))
            .FirstOrDefaultAsync(cancellationToken);

        if (assessmentStrategy is null)
        {
            return Result<GetAssessmentStrategyResponse>.Failure(
                Error.NotFound(
                    "AssessmentStrategy.NotFound",
                    "Assessment strategy was not found for the specified campaign."));
        }

        return Result<GetAssessmentStrategyResponse>.Success(
            assessmentStrategy);
    }
}