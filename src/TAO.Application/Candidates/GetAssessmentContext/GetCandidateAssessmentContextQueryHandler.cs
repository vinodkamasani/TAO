using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.Candidates.GetAssessmentContext;

internal sealed class GetCandidateAssessmentContextQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetCandidateAssessmentContextQuery,
        Result<GetCandidateAssessmentContextResponse>>
{
    public async Task<Result<GetCandidateAssessmentContextResponse>> Handle(
        GetCandidateAssessmentContextQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null)
        {
            return Result<GetCandidateAssessmentContextResponse>.Failure(
                Error.Unauthorized(
                    "CandidateAssessment.Unauthorized",
                    "The current user is not authenticated."));
        }

        var userId = currentUser.UserId.Value;

        // ---------------------------------------------------------
        // 2. Load candidate assessment context
        //
        // Invitation
        //     -> CandidateApplication
        //     -> AssessmentStrategy through CampaignId
        //
        // Candidate ownership is enforced through IdentityUserId.
        // ---------------------------------------------------------

        var assessmentContext = await (
            from invitation in context
                .Set<CandidateInvitation>()

            join candidateApplication in context
                .Set<CandidateApplication>()
                on invitation.CandidateApplicationId
                    equals candidateApplication.Id

            join assessmentStrategy in context
                .Set<AssessmentStrategy>()
                on invitation.CampaignId
                    equals assessmentStrategy.CampaignId

            where invitation.Id == request.InvitationId
                  && candidateApplication.IdentityUserId == userId
                  && assessmentStrategy.Status ==
                     AssessmentStrategyStatus.Approved

            orderby assessmentStrategy.GeneratedOn descending

            select new GetCandidateAssessmentContextResponse(
                candidateApplication.Id,
                assessmentStrategy.Id)
        ).FirstOrDefaultAsync(cancellationToken);

        // ---------------------------------------------------------
        // 3. Context not found
        // ---------------------------------------------------------

        if (assessmentContext is null)
        {
            return Result<GetCandidateAssessmentContextResponse>.Failure(
                Error.NotFound(
                    "CandidateAssessment.ContextNotFound",
                    "The candidate assessment context could not be found."));
        }

        // ---------------------------------------------------------
        // 4. Return context
        // ---------------------------------------------------------

        return Result<GetCandidateAssessmentContextResponse>.Success(
            assessmentContext);
    }
}