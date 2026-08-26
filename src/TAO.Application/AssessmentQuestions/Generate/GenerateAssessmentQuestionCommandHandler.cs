using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Services;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Generate;

internal sealed class GenerateAssessmentQuestionCommandHandler
    : IRequestHandler<
        GenerateAssessmentQuestionCommand,
        Result<GenerateAssessmentQuestionResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAssessmentQuestionGenerationService _generationService;

    public GenerateAssessmentQuestionCommandHandler(
        IApplicationDbContext context,
        IAssessmentQuestionGenerationService generationService)
    {
        _context = context;
        _generationService = generationService;
    }

    public async Task<Result<GenerateAssessmentQuestionResponse>> Handle(
        GenerateAssessmentQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _context
            .Set<AssessmentSession>()
            .FirstOrDefaultAsync(
                x => x.Id == request.AssessmentSessionId,
                cancellationToken);

        if (session is null)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                Error.NotFound(
                    "AssessmentSession.NotFound",
                    $"Assessment session '{request.AssessmentSessionId}' was not found."));
        }

        if (!session.CurrentSessionRoundId.HasValue)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                Error.Validation(
                    "AssessmentSession.NoCurrentRound",
                    "The assessment session does not have a current round."));
        }

        var sessionRound = await _context
            .Set<AssessmentSessionRound>()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == session.CurrentSessionRoundId.Value &&
                    x.AssessmentSessionId == session.Id,
                cancellationToken);

        if (sessionRound is null)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    "The current assessment session round was not found."));
        }

        var result = await _generationService.GenerateNextAsync(
            session,
            sessionRound,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                result.Error!);
        }

        var assessmentQuestion = result.Value!;

        session.SetCurrentQuestion(
            assessmentQuestion.Id);

        _context
            .Set<AssessmentQuestion>()
            .Add(assessmentQuestion);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result<GenerateAssessmentQuestionResponse>.Success(
            new GenerateAssessmentQuestionResponse(
                assessmentQuestion.Id,
                assessmentQuestion.Order,
                assessmentQuestion.PrimaryQuestion,
                assessmentQuestion.Competencies));
    }
}