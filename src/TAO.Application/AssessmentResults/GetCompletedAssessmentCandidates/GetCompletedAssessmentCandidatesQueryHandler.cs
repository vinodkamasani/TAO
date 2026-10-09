using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetCompletedAssessmentCandidates;

internal sealed class GetCompletedAssessmentCandidatesQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetCompletedAssessmentCandidatesQuery,
        Result<
            IReadOnlyCollection<
                GetCompletedAssessmentCandidatesResponse>>>
{
    public async Task<
        Result<
            IReadOnlyCollection<
                GetCompletedAssessmentCandidatesResponse>>> Handle(
        GetCompletedAssessmentCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated)
        {
            return Result<
                IReadOnlyCollection<
                    GetCompletedAssessmentCandidatesResponse>>.Failure(
                Error.Unauthorized(
                    "AssessmentResult.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<
                IReadOnlyCollection<
                    GetCompletedAssessmentCandidatesResponse>>.Failure(
                Error.Unauthorized(
                    "AssessmentResult.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        // ---------------------------------------------------------
        // 3. Validate role
        // ---------------------------------------------------------

        if (currentUser.Role is not
            (UserRole.Recruiter or UserRole.HiringManager))
        {
            return Result<
                IReadOnlyCollection<
                    GetCompletedAssessmentCandidatesResponse>>.Failure(
                Error.Forbidden(
                    "AssessmentResult.Forbidden",
                    "The current user does not have permission to view assessment results."));
        }

        // ---------------------------------------------------------
        // 4. Load candidates who completed the assessment
        //
        // Organization isolation is enforced using the
        // authenticated user's organization.
        // ---------------------------------------------------------

        var candidates = await (
            from candidateApplication in context
                .Set<CandidateApplication>()
                .AsNoTracking()

            join assessmentSession in context
                .Set<AssessmentSession>()
                .AsNoTracking()
                on candidateApplication.Id
                    equals assessmentSession.CandidateApplicationId

            where candidateApplication.CampaignId ==
                    request.CampaignId
                  && candidateApplication.OrganizationId ==
                    organizationId.Value
                  && assessmentSession.Status ==
                    AssessmentSessionStatus.Completed

            orderby assessmentSession.CompletedOn descending

            select new GetCompletedAssessmentCandidatesResponse(
                candidateApplication.Id,
                candidateApplication.CandidateName,
                candidateApplication.Email,
                candidateApplication.Phone,
                candidateApplication.LinkedInUrl,
                candidateApplication.CurrentCompany,
                candidateApplication.CurrentLocation,
                assessmentSession.Id,
                assessmentSession.CompletedOn!.Value))
            .ToListAsync(cancellationToken);

        return Result<
            IReadOnlyCollection<
                GetCompletedAssessmentCandidatesResponse>>.Success(
            candidates);
    }
}