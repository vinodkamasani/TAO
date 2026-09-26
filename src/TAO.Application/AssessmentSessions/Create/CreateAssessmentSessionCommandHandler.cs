using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentSessions.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Create;

internal sealed class CreateAssessmentSessionCommandHandler
    : IRequestHandler<
        CreateAssessmentSessionCommand,
        Result<AssessmentSessionResponse>>
{
    private const int SessionExpiryHours = 24;
    private const int ConsentVersion = 1;

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateAssessmentSessionCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<AssessmentSessionResponse>> Handle(
        CreateAssessmentSessionCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentSession.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentSession.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        // ---------------------------------------------------------
        // 3. Load Candidate Application
        //
        // IMPORTANT:
        // OrganizationId comes from the authenticated user's claims.
        // Never trust OrganizationId from the request.
        // ---------------------------------------------------------

        var candidateApplication = await _context
            .Set<CandidateApplication>()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.CandidateApplicationId
                    && x.OrganizationId == organizationId.Value,
                cancellationToken);

        if (candidateApplication is null)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.NotFound(
                    "CandidateApplication.NotFound",
                    $"Candidate Application '{request.CandidateApplicationId}' was not found."));
        }

        // ---------------------------------------------------------
        // 4. Load Assessment Strategy
        //
        // Also enforce tenant isolation here.
        // ---------------------------------------------------------

        var assessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.AssessmentStrategyId
                    && x.OrganizationId == organizationId.Value,
                cancellationToken);

        if (assessmentStrategy is null)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.NotFound(
                    "AssessmentStrategy.NotFound",
                    $"Assessment Strategy '{request.AssessmentStrategyId}' was not found."));
        }

        // ---------------------------------------------------------
        // 5. Validate Assessment Strategy status
        // ---------------------------------------------------------

        if (assessmentStrategy.Status != AssessmentStrategyStatus.Approved)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentStrategy.NotApproved",
                    "Only an approved Assessment Strategy can be used to create an Assessment Session."));
        }

        // ---------------------------------------------------------
        // 6. Ensure candidate and strategy belong to the same
        //    organization.
        //
        // Normally this is guaranteed by the two queries above,
        // but this explicit check protects the business invariant.
        // ---------------------------------------------------------

        if (candidateApplication.OrganizationId
            != assessmentStrategy.OrganizationId)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentSession.OrganizationMismatch",
                    "The Candidate Application and Assessment Strategy must belong to the same organization."));
        }

        // ---------------------------------------------------------
        // 7. Check for an existing active session
        // ---------------------------------------------------------

        var existingSession = await _context
            .Set<AssessmentSession>()
            .AnyAsync(
                x =>
                    x.CandidateApplicationId
                        == request.CandidateApplicationId
                    && x.AssessmentStrategyId
                        == request.AssessmentStrategyId
                    && x.Status != AssessmentSessionStatus.Terminated
                    && x.Status != AssessmentSessionStatus.Expired,
                cancellationToken);

        if (existingSession)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.Conflict(
                    "AssessmentSession.AlreadyExists",
                    "An active Assessment Session already exists for this candidate and assessment."));
        }

        // ---------------------------------------------------------
        // 8. Load Assessment Rounds
        // ---------------------------------------------------------

        var assessmentRounds = await _context
            .Set<AssessmentRound>()
            .Where(x =>
                x.AssessmentStrategyId == assessmentStrategy.Id)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        if (assessmentRounds.Count == 0)
        {
            return Result<AssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentStrategy.NoRounds",
                    "The approved Assessment Strategy must contain at least one Assessment Round."));
        }

        // ---------------------------------------------------------
        // 9. Create Strategy Snapshot
        // ---------------------------------------------------------

        var strategySnapshot = AssessmentStrategySnapshot.Create(
            assessmentStrategy.StructuredContent.Value);

        // ---------------------------------------------------------
        // 10. Calculate session expiry
        // ---------------------------------------------------------

        var expiresOn = DateTime.UtcNow.AddHours(
            SessionExpiryHours);

        // ---------------------------------------------------------
        // 11. Create Assessment Session
        // ---------------------------------------------------------

        var assessmentSession = AssessmentSession.Create(
            candidateApplication.Id,
            assessmentStrategy.Id,
            strategySnapshot,
            expiresOn);

        // ---------------------------------------------------------
        // 12. Accept consent
        //
        // Temporary MVP assumption:
        // candidate has already accepted the assessment and consent.
        // ---------------------------------------------------------

        assessmentSession.AcceptConsent(
            ConsentVersion);

        _context
            .Set<AssessmentSession>()
            .Add(assessmentSession);

        // ---------------------------------------------------------
        // 13. Create Assessment Session Rounds
        // ---------------------------------------------------------

        foreach (var assessmentRound in assessmentRounds)
        {
            var sessionRound = AssessmentSessionRound.Create(
                assessmentSession.Id,
                assessmentRound);

            _context
                .Set<AssessmentSessionRound>()
                .Add(sessionRound);
        }

        // ---------------------------------------------------------
        // 14. Persist session and rounds
        // ---------------------------------------------------------

        await _context.SaveChangesAsync(
            cancellationToken);

        // ---------------------------------------------------------
        // 15. Reload persisted session rounds
        //
        // We deliberately materialize the entities first.
        //
        // Do NOT project directly into the response DTO here because
        // nested enum.ToString() expressions cannot reliably be
        // translated by EF Core.
        // ---------------------------------------------------------

        var sessionRounds = await _context
            .Set<AssessmentSessionRound>()
            .AsNoTracking()
            .Where(x =>
                x.AssessmentSessionId == assessmentSession.Id)
            .Include(x => x.Questions)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // 16. Map rounds to API response in memory
        // ---------------------------------------------------------

        var roundResponses = sessionRounds
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
        // 17. Build complete Assessment Session response
        // ---------------------------------------------------------

        var response = new AssessmentSessionResponse(
            assessmentSession.Id,
            assessmentSession.CandidateApplicationId,
            assessmentSession.AssessmentStrategyId,
            assessmentSession.Status.ToString(),
            assessmentSession.StrategySnapshot,
            assessmentSession.CurrentSessionRoundId,
            assessmentSession.CurrentQuestionId,
            assessmentSession.ConsentAcceptedOn,
            assessmentSession.ConsentVersion,
            assessmentSession.StartedOn,
            assessmentSession.CompletedOn,
            assessmentSession.AssessmentExpiresOn,
            assessmentSession.LastActivityOn,
            assessmentSession.HasUsedInterruptionWindow,
            assessmentSession.IsInterrupted,
            roundResponses);

        // ---------------------------------------------------------
        // 18. Return complete response
        // ---------------------------------------------------------

        return Result<AssessmentSessionResponse>.Success(
            response);
    }
}