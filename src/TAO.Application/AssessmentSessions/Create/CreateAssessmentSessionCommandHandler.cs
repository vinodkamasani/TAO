using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Create;

internal sealed class CreateAssessmentSessionCommandHandler
    : IRequestHandler<CreateAssessmentSessionCommand, Result<Guid>>
{
    private const int SessionExpiryHours = 24;
    private const int ConsentVersion = 1;

    private readonly IApplicationDbContext _context;

    public CreateAssessmentSessionCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateAssessmentSessionCommand request,
        CancellationToken cancellationToken)
    {
        var candidateApplication = await _context
            .Set<CandidateApplication>()
            .FirstOrDefaultAsync(
                x => x.Id == request.CandidateApplicationId,
                cancellationToken);

        if (candidateApplication is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "CandidateApplication.NotFound",
                    $"Candidate Application '{request.CandidateApplicationId}' was not found."));
        }

        var assessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentStrategyId,
                cancellationToken);

        if (assessmentStrategy is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "AssessmentStrategy.NotFound",
                    $"Assessment Strategy '{request.AssessmentStrategyId}' was not found."));
        }

        if (assessmentStrategy.Status != AssessmentStrategyStatus.Approved)
        {
            return Result<Guid>.Failure(
                Error.Validation(
                    "AssessmentStrategy.NotApproved",
                    "Only an approved Assessment Strategy can be used to create an Assessment Session."));
        }

        var existingSession = await _context
            .Set<AssessmentSession>()
            .AnyAsync(
                x =>
                    x.CandidateApplicationId == request.CandidateApplicationId
                    && x.AssessmentStrategyId == request.AssessmentStrategyId
                    && x.Status != AssessmentSessionStatus.Terminated
                    && x.Status != AssessmentSessionStatus.Expired,
                cancellationToken);

        if (existingSession)
        {
            return Result<Guid>.Failure(
                Error.Conflict(
                    "AssessmentSession.AlreadyExists",
                    "An active Assessment Session already exists for this candidate and assessment."));
        }

        var strategySnapshot = AssessmentStrategySnapshot.Create(
            assessmentStrategy.StructuredContent.Value);

        var expiresOn = DateTime.UtcNow.AddHours(
            SessionExpiryHours);

        var assessmentSession = AssessmentSession.Create(
            candidateApplication.Id,
            assessmentStrategy.Id,
            strategySnapshot,
            expiresOn);

        // Temporary MVP assumption:
        // candidate has already accepted the assessment and consent.
        assessmentSession.AcceptConsent(
            ConsentVersion);

        _context
            .Set<AssessmentSession>()
            .Add(assessmentSession);

        var assessmentRounds = await _context
            .Set<AssessmentRound>()
            .Where(x =>
                x.AssessmentStrategyId == assessmentStrategy.Id)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        if (assessmentRounds.Count == 0)
        {
            return Result<Guid>.Failure(
                Error.Validation(
                    "AssessmentStrategy.NoRounds",
                    "The approved Assessment Strategy must contain at least one Assessment Round."));
        }

        foreach (var assessmentRound in assessmentRounds)
        {
            var sessionRound = AssessmentSessionRound.Create(
                assessmentSession.Id,
                assessmentRound);

            _context
                .Set<AssessmentSessionRound>()
                .Add(sessionRound);
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            assessmentSession.Id);
    }
}