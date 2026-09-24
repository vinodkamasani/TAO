
using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Application.HiringStrategies.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Get;

internal class GetHiringStrategyQueryHandler: IRequestHandler<GetHiringStrategyQuery, Result<HiringStrategyResponse>>
{
    private readonly IApplicationDbContext _context;
    public GetHiringStrategyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<HiringStrategyResponse>> Handle(GetHiringStrategyQuery request, CancellationToken cancellationToken)
    {
        var hiringStrategy = await _context
                    .Set<HiringStrategy>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        hs => hs.CampaignId == request.CampaignId,
                        cancellationToken);


        if(hiringStrategy is null)
        return Result<HiringStrategyResponse>.Failure(
            Error.NotFound(
                "HiringStrategy.NotFound",
                $"No Hiring Strategy found for Campaign '{request.CampaignId}'."));


        var response = new HiringStrategyResponse(
                  hiringStrategy.Id,
                  hiringStrategy.OrganizationId,
                  hiringStrategy.CampaignId,
                  hiringStrategy.Content,
                  hiringStrategy.StructuredContent,
                  hiringStrategy.Status.ToString(),
                  hiringStrategy.ProviderName,
                  hiringStrategy.ModelName,
                  hiringStrategy.PromptVersion,
                  hiringStrategy.CreatedOn);

        return Result<HiringStrategyResponse>.Success(response);

    }
}
