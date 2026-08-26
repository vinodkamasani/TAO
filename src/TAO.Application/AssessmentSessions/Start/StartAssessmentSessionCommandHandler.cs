using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentSessions.Start;

internal sealed class StartAssessmentSessionCommandHandler
    : IRequestHandler<StartAssessmentSessionCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public StartAssessmentSessionCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        StartAssessmentSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _context
            .Set<AssessmentSession>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentSessionId,
                cancellationToken);

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