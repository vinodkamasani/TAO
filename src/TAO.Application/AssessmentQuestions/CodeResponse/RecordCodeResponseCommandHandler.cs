using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.CodeResponse;

internal sealed class RecordCodeResponseCommandHandler
    : IRequestHandler<
        RecordCodeResponseCommand,
        Result>
{
    private readonly IApplicationDbContext _context;

    public RecordCodeResponseCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        RecordCodeResponseCommand request,
        CancellationToken cancellationToken)
    {
        var question = await _context
            .Set<AssessmentQuestion>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentQuestionId,
                cancellationToken);

        if (question is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.AssessmentQuestionId}' was not found."));
        }

        if (question.Status != AssessmentQuestionStatus.InProgress)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.NotInProgress",
                    "Candidate code can only be recorded for an in-progress assessment question."));
        }

        var sessionRound = await _context
            .Set<AssessmentSessionRound>()
            .FirstOrDefaultAsync(
                x => x.Id == question.AssessmentSessionRoundId,
                cancellationToken);

        if (sessionRound is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    $"Assessment session round '{question.AssessmentSessionRoundId}' was not found."));
        }

        if (sessionRound.Type is not AssessmentRoundType.Coding
            and not AssessmentRoundType.Dsa)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.CodeNotAllowed",
                    "Candidate code can only be recorded for Coding or DSA assessment rounds."));
        }

        question.SetCandidateCode(request.Code);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}