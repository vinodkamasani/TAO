
using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Get;

internal class GetHiringStrategyQueryHandler: IRequestHandler<GetHiringStrategyQuery, Result<GetHiringStrategyResponse>>
{
    private readonly IApplicationDbContext _context;
    public GetHiringStrategyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<GetHiringStrategyResponse>> Handle(GetHiringStrategyQuery request, CancellationToken cancellationToken)
    {
        var hiringStrategy = await _context
                    .Set<HiringStrategy>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        hs => hs.CampaignId == request.CampaignId,
                        cancellationToken);


        if(hiringStrategy is null)
        return Result<GetHiringStrategyResponse>.Failure(
            Error.NotFound(
                "HiringStrategy.NotFound",
                $"No Hiring Strategy found for Campaign '{request.CampaignId}'."));

                
        var response = new GetHiringStrategyResponse
        {
            Id = hiringStrategy.Id,
            CampaignId = hiringStrategy.CampaignId,
            GeneratedContent = hiringStrategy.Content,
            StructuredContent = hiringStrategy.StructuredContent,
            Status = hiringStrategy.Status,
            ProviderName = hiringStrategy.ProviderName,
            ModelName = hiringStrategy.ModelName,
            PromptVersion = hiringStrategy.PromptVersion,
            CreatedOnUtc = hiringStrategy.CreatedOn
        };
        return Result<GetHiringStrategyResponse>.Success(response);

    }
}
