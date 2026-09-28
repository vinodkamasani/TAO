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
    private readonly ICurrentUser _currentUser;

    public RecordCodeResponseCommandHandler(
        IApplicationDbContext context,
        ISender sender,
        ICurrentUser currentUser)
    {
        _context = context;
        _sender = sender;
        _currentUser = currentUser;
    }

    public async Task<Result<GenerateFollowUpResponse>> Handle(
        RecordCodeResponseCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated ||
           _currentUser.UserId is null ||
           _currentUser.OrganizationId is null)
        {
            return Result<GenerateFollowUpResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentEvaluation.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId = _currentUser.OrganizationId.Value;

        var question = await (
      from q in _context.Set<AssessmentQuestion>()

      join sessionRoundLocal in _context.Set<AssessmentSessionRound>()
          on q.AssessmentSessionRoundId equals sessionRoundLocal.Id

      join session in _context.Set<AssessmentSession>()
          on sessionRoundLocal.AssessmentSessionId equals session.Id

      join candidateApplication in _context.Set<CandidateApplication>()
          on session.CandidateApplicationId equals candidateApplication.Id

      where q.Id == request.AssessmentQuestionId
            && candidateApplication.OrganizationId == organizationId

      select q
  ).FirstOrDefaultAsync(cancellationToken);


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