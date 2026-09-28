
using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Generate;
using TAO.Application.Common.Interfaces;
using TAO.Application.HiringStrategies.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.Application.HiringStrategies.Get;

internal class GetHiringStrategyQueryHandler: IRequestHandler<GetHiringStrategyQuery, Result<HiringStrategyResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    public GetHiringStrategyQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<Result<HiringStrategyResponse>> Handle(GetHiringStrategyQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result<HiringStrategyResponse>.Failure(
                Error.Unauthorized(
                    "HiringStrategy.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<HiringStrategyResponse>.Failure(
                Error.Unauthorized(
                    "HiringStrategy.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }



        var hiringStrategy = await _context
                    .Set<HiringStrategy>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        hs => hs.CampaignId == request.CampaignId && hs.OrganizationId == organizationId.Value,
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
