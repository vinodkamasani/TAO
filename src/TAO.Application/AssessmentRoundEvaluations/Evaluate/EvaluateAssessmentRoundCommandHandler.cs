using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.Application.AssessmentQuestions.Generate;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentRoundEvaluations.Evaluate;

internal sealed class EvaluateAssessmentRoundCommandHandler
    : IRequestHandler<EvaluateAssessmentRoundCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IAssessmentRoundEvaluationGenerator _generator;
    private readonly ICurrentUser _currentUser;

    public EvaluateAssessmentRoundCommandHandler(
        IApplicationDbContext context,
        IAssessmentRoundEvaluationGenerator generator,
        ICurrentUser currentUser)
    {
        _context = context;
        _generator = generator;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        EvaluateAssessmentRoundCommand request,
        CancellationToken cancellationToken)
    {

        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<Result>.Failure(
                Error.Unauthorized(
                    "AssessmentQuestion.Unauthorized",
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

        var sessionRound = await (
     from round in _context.Set<AssessmentSessionRound>()

     join session in _context.Set<AssessmentSession>()
         on round.AssessmentSessionId equals session.Id

     join candidateApplication in _context.Set<CandidateApplication>()
         on session.CandidateApplicationId equals candidateApplication.Id

     where round.Id == request.AssessmentSessionRoundId
           && candidateApplication.OrganizationId == organizationId

     select round
 ).FirstOrDefaultAsync(cancellationToken);
        if (sessionRound is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    $"Assessment session round '{request.AssessmentSessionRoundId}' was not found."));
        }

        var evaluations = await _context
            .Set<AssessmentQuestionEvaluation>()
            .Where(x =>
                _context
                    .Set<AssessmentQuestion>()
                    .Any(q =>
                        q.Id == x.AssessmentQuestionId &&
                        q.AssessmentSessionRoundId ==
                            sessionRound.Id))
            .ToListAsync(
                cancellationToken);


        // All questions in the round were skipped.
        if (evaluations.Count == 0)
        {
            var skippedEvaluation =
                AssessmentRoundEvaluation.Create(
                    sessionRound.Id,
                    0,
                    0,
                    [],
                    ["Candidate skipped all questions in this round."],
                    []);

            _context
                .Set<AssessmentRoundEvaluation>()
                .Add(skippedEvaluation);

            await _context.SaveChangesAsync(
                cancellationToken);

            return Result.Success();
        }

        var generationResult =
            await _generator.GenerateAsync(
                sessionRound,
                evaluations,
                cancellationToken);

        if (generationResult.IsFailure)
        {
            return Result.Failure(
                generationResult.Error!);
        }

        var result = generationResult.Value;

        var score = (byte)Math.Round(
            evaluations.Average(x => x.Score),
            MidpointRounding.AwayFromZero);

        var existingEvaluation =
            await _context
                .Set<AssessmentRoundEvaluation>()
                .FirstOrDefaultAsync(
                    x =>
                        x.AssessmentSessionRoundId ==
                        sessionRound.Id,
                    cancellationToken);

        if (existingEvaluation is null)
        {
            var evaluation =
                AssessmentRoundEvaluation.Create(
                    sessionRound.Id,
                    score,
                    result.Confidence,
                    result.Strengths,
                    result.Gaps,
                    result.Evidence);

            _context
                .Set<AssessmentRoundEvaluation>()
                .Add(evaluation);
        }
        else
        {
            existingEvaluation.Update(
                score,
                result.Confidence,
                result.Strengths,
                result.Gaps,
                result.Evidence);
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}