using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentSessions.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Get;

public sealed class GetAssessmentSessionQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetAssessmentSessionQuery,
        Result<AssessmentSessionResponse>>
{
    public async Task<Result<AssessmentSessionResponse>> Handle(
        GetAssessmentSessionQuery request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentSession.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get current user's organization
        // ---------------------------------------------------------

        var organizationId = currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentSession.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        // ---------------------------------------------------------
        // 3. Load assessment session with tenant isolation
        //
        // AssessmentSession does not contain OrganizationId.
        // Therefore, tenant isolation is enforced through
        // CandidateApplication.
        // ---------------------------------------------------------

        var session = await (
            from assessmentSession in context
                .Set<AssessmentSession>()
                .AsNoTracking()

            join candidateApplication in context
                .Set<CandidateApplication>()
                .AsNoTracking()
                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentSession.Id == request.AssessmentSessionId
                  && candidateApplication.OrganizationId
                      == organizationId.Value

            select assessmentSession
        ).FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.NotFound(
                    "AssessmentSession.NotFound",
                    "The assessment session could not be found."));
        }

        // ---------------------------------------------------------
        // 4. Load assessment session rounds
        //
        // Materialize first. Do not project enum.ToString()
        // or nested value objects inside the EF query.
        // ---------------------------------------------------------

        var rounds = await context
            .Set<AssessmentSessionRound>()
            .AsNoTracking()
            .Where(x =>
                x.AssessmentSessionId == session.Id)
            .Include(x => x.Questions)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // 5. Map entities to API response
        //
        // This mapping happens in memory, so enum.ToString()
        // and nested collections are safe.
        // ---------------------------------------------------------

        var roundResponses = rounds
            .Select(x =>
                new AssessmentSessionRoundResponse(
                    x.Id,
                    x.AssessmentRoundId,
                    x.Order,
                    x.Type.ToString(),
                    x.Difficulty.ToString(),
                    x.DurationInMinutes,
                    x.TargetQuestionCount,
                    x.Status.ToString(),
                    x.StartedOn,
                    x.ExpiresOn,
                    x.CompletedOn,
                    x.Competencies
                        .Select(c =>
                            new AssessmentRoundCompetencyResponse(
                                c.Name,
                                c.Priority.ToString(),
                                c.MinimumPassPercentage))
                        .ToList(),
                    x.Questions
                        .OrderBy(q => q.Order)
                        .Select(q =>
                            new AssessmentQuestionResponse(
                                q.Id,
                                q.Order,
                                q.PrimaryQuestion,
                                q.Status.ToString(),
                                q.Competencies.ToList(),
                                q.Conversation,
                                q.CandidateCode,
                                q.StartedOn,
                                q.CompletedOn))
                        .ToList()))
            .ToList();

        // ---------------------------------------------------------
        // 6. Build final response
        // ---------------------------------------------------------

        var response = new AssessmentSessionResponse(
            session.Id,
            session.CandidateApplicationId,
            session.AssessmentStrategyId,
            session.Status.ToString(),
            session.StrategySnapshot,
            session.CurrentSessionRoundId,
            session.CurrentQuestionId,
            session.ConsentAcceptedOn,
            session.ConsentVersion,
            session.StartedOn,
            session.CompletedOn,
            session.AssessmentExpiresOn,
            session.LastActivityOn,
            session.HasUsedInterruptionWindow,
            session.IsInterrupted,
            roundResponses);

        // ---------------------------------------------------------
        // 7. Return response
        // ---------------------------------------------------------

        return Result<AssessmentSessionResponse>.Success(
            response);
    }
}