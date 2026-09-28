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
    : IRequestHandler<SkipAssessmentQuestionCommand, Result>
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

    public async Task<Result> Handle(
        SkipAssessmentQuestionCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null ||
            _currentUser.OrganizationId is null)
        {
            return Result<Result>.Failure(
                Error.Unauthorized(
                    "AssessmentEvaluation.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId = _currentUser.OrganizationId.Value;

        var question = await (
      from q in _context.Set<AssessmentQuestion>()

      join sessionRoundLocal in _context.Set<AssessmentSessionRound>()
          on q.AssessmentSessionRoundId equals sessionRoundLocal.Id

      join sessionLocal in _context.Set<AssessmentSession>()
          on sessionRoundLocal.AssessmentSessionId equals sessionLocal.Id

      join candidateApplication in _context.Set<CandidateApplication>()
          on sessionLocal.CandidateApplicationId equals candidateApplication.Id

      where q.Id == request.AssessmentQuestionId
            && candidateApplication.OrganizationId == organizationId

      select q
  ).FirstOrDefaultAsync(cancellationToken);

        if (question is null)
        {
            return Result.Failure(
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

        if (session is null)
        {
            return Result.Failure(
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
        catch (InvalidOperationException ex)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.CannotSkip",
                    ex.Message));
        }
    }
}