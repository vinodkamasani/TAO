using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.FollowUp;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.CodeResponse;

internal sealed class RecordCodeResponseCommandHandler
    : IRequestHandler<
        RecordCodeResponseCommand,
        Result<GenerateFollowUpResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;

    public RecordCodeResponseCommandHandler(
        IApplicationDbContext context,
        ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<Result<GenerateFollowUpResponse>> Handle(
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
            return Result<GenerateFollowUpResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.AssessmentQuestionId}' was not found."));
        }

        if (question.Status != AssessmentQuestionStatus.InProgress)
        {
            return Result<GenerateFollowUpResponse>.Failure(
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
            return Result<GenerateFollowUpResponse>.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    $"Assessment session round '{question.AssessmentSessionRoundId}' was not found."));
        }

        if (sessionRound.Type is not AssessmentRoundType.Coding
            and not AssessmentRoundType.Dsa)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.CodeNotAllowed",
                    "Candidate code can only be recorded for Coding or DSA assessment rounds."));
        }

        question.SetCandidateCode(request.Code);

        await _context.SaveChangesAsync(
            cancellationToken);

        var followUpResult = await _sender.Send(
     new GenerateFollowUpCommand(question.Id),
     cancellationToken);

        if (followUpResult.IsFailure &&
            followUpResult.Error?.Code ==
                "AssessmentQuestion.FollowUpLimitReached")
        {
            return Result<GenerateFollowUpResponse>.Success(
                new GenerateFollowUpResponse(null));
        }

        return followUpResult;
    }
}