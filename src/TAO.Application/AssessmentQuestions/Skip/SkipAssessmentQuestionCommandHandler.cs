using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.FollowUp;
using TAO.Application.AssessmentSessions.Advance;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Skip;

internal sealed class SkipAssessmentQuestionCommandHandler
    : IRequestHandler<SkipAssessmentQuestionCommand, Result<AdvanceAssessmentSessionResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    private readonly ICurrentUser _currentUser;

    public SkipAssessmentQuestionCommandHandler(
        IApplicationDbContext context,
        ISender sender,
        ICurrentUser currentUser)
    {
        _context = context;
        _sender = sender;
        _currentUser = currentUser;
    }

    public async Task<Result<AdvanceAssessmentSessionResponse>> Handle(
        SkipAssessmentQuestionCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null )
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentEvaluation.Unauthorized",
                    "The current user is not authenticated."));
        }

        var userId = _currentUser.UserId.Value;

        var question = await (
      from q in _context.Set<AssessmentQuestion>()

      join sessionRoundLocal in _context.Set<AssessmentSessionRound>()
          on q.AssessmentSessionRoundId equals sessionRoundLocal.Id

      join sessionLocal in _context.Set<AssessmentSession>()
          on sessionRoundLocal.AssessmentSessionId equals sessionLocal.Id

      join candidateApplication in _context.Set<CandidateApplication>()
          on sessionLocal.CandidateApplicationId equals candidateApplication.Id

      where q.Id == request.AssessmentQuestionId
            && candidateApplication.IdentityUserId == userId

      select q
  ).FirstOrDefaultAsync(cancellationToken);

        // when question is null
        if (question is null)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.AssessmentQuestionId}' was not found."));
        }

        var session = await _context
            .Set<AssessmentSession>()
            .FirstOrDefaultAsync(
                x =>
                    x.CurrentQuestionId == question.Id &&
                    x.CurrentSessionRoundId == question.AssessmentSessionRoundId,
                cancellationToken);

        // when session is null
        if (session is null)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                    Error.Validation(
                    "AssessmentQuestion.NotCurrentQuestion",
                    "The assessment question is not the current question for an assessment session."));
        }

        try
        {
            question.Skip(DateTime.UtcNow);

            await _context.SaveChangesAsync(
                cancellationToken);

            return await _sender.Send(
                new AdvanceAssessmentSessionCommand(
                    session.Id),
                cancellationToken);
        }
        // in the catch
        catch (InvalidOperationException ex)
        {
            return Result<AdvanceAssessmentSessionResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.CannotSkip",
                    ex.Message));
        }
    }
}