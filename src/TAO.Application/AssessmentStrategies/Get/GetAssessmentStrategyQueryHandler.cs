using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Generate;
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
    private readonly ICurrentUser _currentUser;

    public GetAssessmentStrategyQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<GetAssessmentStrategyResponse>> Handle(
        GetAssessmentStrategyQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<GetAssessmentStrategyResponse>.Failure(
                Error.Unauthorized(
                    "GetAssessmentStrategyResponse.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<GetAssessmentStrategyResponse>.Failure(
                Error.Unauthorized(
                    "GetAssessmentStrategyResponse.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }


        var assessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .AsNoTracking()
            .Where(x => x.CampaignId == request.CampaignId && x.OrganizationId == _currentUser.OrganizationId)
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