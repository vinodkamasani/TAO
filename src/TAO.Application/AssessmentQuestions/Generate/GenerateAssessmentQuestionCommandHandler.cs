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
    private readonly ICurrentUser _currentUser;
    private readonly IAssessmentQuestionGenerationService _generationService;

    public GenerateAssessmentQuestionCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IAssessmentQuestionGenerationService generationService)
    {
        _context = context;
        _currentUser = currentUser;
        _generationService = generationService;
    }

    public async Task<Result<GenerateAssessmentQuestionResponse>> Handle(
        GenerateAssessmentQuestionCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
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
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                Error.NotFound(
                    "AssessmentSession.NotFound",
                    $"Assessment session '{request.AssessmentSessionId}' was not found."));
        }

        // ---------------------------------------------------------
        // 4. Validate current round
        // ---------------------------------------------------------

        if (!session.CurrentSessionRoundId.HasValue)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                Error.Validation(
                    "AssessmentSession.NoCurrentRound",
                    "The assessment session does not have a current round."));
        }

        // ---------------------------------------------------------
        // 5. Load current session round
        // ---------------------------------------------------------

        var sessionRound = await _context
            .Set<AssessmentSessionRound>()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == session.CurrentSessionRoundId.Value
                    && x.AssessmentSessionId == session.Id,
                cancellationToken);

        if (sessionRound is null)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    "The current assessment session round was not found."));
        }

        // ---------------------------------------------------------
        // 6. Generate next question
        // ---------------------------------------------------------

        var generationResult =
            await _generationService.GenerateNextAsync(
                session,
                sessionRound,
                cancellationToken);

        if (generationResult.IsFailure)
        {
            return Result<GenerateAssessmentQuestionResponse>.Failure(
                generationResult.Error!);
        }

        var assessmentQuestion = generationResult.Value!;

        // ---------------------------------------------------------
        // 7. Set current question
        // ---------------------------------------------------------

        session.SetCurrentQuestion(
            assessmentQuestion.Id);

        // ---------------------------------------------------------
        // 8. Persist generated question
        // ---------------------------------------------------------

        _context
            .Set<AssessmentQuestion>()
            .Add(assessmentQuestion);

        await _context.SaveChangesAsync(
            cancellationToken);

        // ---------------------------------------------------------
        // 9. Return generated question
        // ---------------------------------------------------------

        var response = new GenerateAssessmentQuestionResponse(
            assessmentQuestion.Id,
            assessmentQuestion.Order,
            assessmentQuestion.PrimaryQuestion,
            assessmentQuestion.Competencies);

        return Result<GenerateAssessmentQuestionResponse>.Success(
            response);
    }
}