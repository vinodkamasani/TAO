using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Generate;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Start;

internal sealed class StartAssessmentSessionCommandHandler
    : IRequestHandler<StartAssessmentSessionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public StartAssessmentSessionCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        StartAssessmentSessionCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<Result>.Failure(
                Error.Unauthorized(
                    "StartAssessmentSessionCommandHandler.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentQuestion.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        // ---------------------------------------------------------
        // 3. Load assessment session with tenant isolation
        //
        // AssessmentSession does not contain OrganizationId.
        // Scope it through CandidateApplication.
        // ---------------------------------------------------------

        var session = await (
                  from assessmentSession in _context
                      .Set<AssessmentSession>()

                  join candidateApplication in _context
                      .Set<CandidateApplication>()
                      on assessmentSession.CandidateApplicationId
                          equals candidateApplication.Id

                  where assessmentSession.Id == request.AssessmentSessionId
                        && candidateApplication.OrganizationId
                            == organizationId.Value

                  select assessmentSession
              ).FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentSession.NotFound",
                    $"Assessment session '{request.AssessmentSessionId}' was not found."));
        }

        var sessionRound = await _context
            .Set<AssessmentSessionRound>()
            .Where(x =>
                x.AssessmentSessionId == session.Id)
            .OrderBy(x => x.Order)
            .FirstOrDefaultAsync(
                cancellationToken);

        if (sessionRound is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentSession.NoRounds",
                    "The assessment session does not contain any assessment rounds."));
        }

        try
        {
            var startedOn = DateTime.UtcNow;

            session.Start();

            session.SetCurrentRound(
                sessionRound.Id);

            sessionRound.Start(
                startedOn,
                startedOn.AddMinutes(
                    sessionRound.DurationInMinutes));

            await _context.SaveChangesAsync(
                cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentSession.CannotStart",
                    ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentSession.CannotStart",
                    ex.Message));
        }
    }
}