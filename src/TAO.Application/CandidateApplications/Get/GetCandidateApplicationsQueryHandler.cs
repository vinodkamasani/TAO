using MediatR;
using Microsoft.EntityFrameworkCore;
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

    public GetCandidateApplicationsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyCollection<GetCandidateApplicationsResponse>>> Handle(
        GetCandidateApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var candidates = await _context
            .Set<CandidateApplication>()
            .AsNoTracking()
            .Where(x => x.CampaignId == request.CampaignId)
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