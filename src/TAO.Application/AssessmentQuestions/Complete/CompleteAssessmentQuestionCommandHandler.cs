using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestionEvaluations.Evaluate;
using TAO.Application.AssessmentSessions.Advance;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Complete;

internal sealed class CompleteAssessmentQuestionCommandHandler
    : IRequestHandler<CompleteAssessmentQuestionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;

    public CompleteAssessmentQuestionCommandHandler(
        IApplicationDbContext context,
        ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<Result> Handle(
        CompleteAssessmentQuestionCommand request,
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
            question.Complete(DateTime.UtcNow);

            await _context.SaveChangesAsync(
                cancellationToken);

            var evaluationResult = await _sender.Send(
              new EvaluateAssessmentQuestionCommand(
                  question.Id),
              cancellationToken);

            if (evaluationResult.IsFailure)
            {
                return Result.Failure(
                    evaluationResult.Error!);
            }

            return await _sender.Send(
                new AdvanceAssessmentSessionCommand(
                    session.Id),
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.CannotComplete",
                    ex.Message));
        }
    }
}