using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Generate;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.CandidateApplications.Get;

public sealed class GetCandidateApplicationsQueryHandler
    : IRequestHandler<
        GetCandidateApplicationsQuery,
        Result<IReadOnlyCollection<GetCandidateApplicationsResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetCandidateApplicationsQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyCollection<GetCandidateApplicationsResponse>>> Handle(
        GetCandidateApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result<IReadOnlyCollection<GetCandidateApplicationsResponse>>.Failure(
                Error.Unauthorized(
                    "CandidateApplications.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<IReadOnlyCollection<GetCandidateApplicationsResponse>>.Failure(
                Error.Unauthorized(
                    "CandidateApplications.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }


        var candidates = await _context
            .Set<CandidateApplication>()
            .AsNoTracking()
            .Where(x => x.CampaignId == request.CampaignId && x.OrganizationId == organizationId.Value)
            .OrderBy(x => x.CandidateName)
            .Select(x => new GetCandidateApplicationsResponse(
                x.Id,
                x.CandidateName,
                x.Email,
                x.Phone,
                x.LinkedInUrl,
                x.CurrentCompany,
                x.CurrentLocation,
                x.OverallMatchPercentage,
                x.IsRecommended
                    ? "Approved"
                    : "Rejected",
                x.ResumeUploadedOn,
                x.LastScreenedOn))
            .ToListAsync(cancellationToken);

        return Result<
            IReadOnlyCollection<GetCandidateApplicationsResponse>>
            .Success(candidates);
    }
}