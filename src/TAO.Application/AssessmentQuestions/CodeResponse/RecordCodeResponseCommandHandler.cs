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
    private readonly ICurrentUser _currentUser;

    public RecordCodeResponseCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        RecordCodeResponseCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated ||
            _currentUser.UserId is null ||
            _currentUser.OrganizationId is null)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "AssessmentQuestion.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId =
            _currentUser.OrganizationId.Value;

        // ---------------------------------------------------------
        // 2. Load question with tenant isolation
        //
        // AssessmentQuestion does not have OrganizationId.
        //
        // Question
        //    -> SessionRound
        //    -> Session
        //    -> CandidateApplication
        //    -> Organization
        // ---------------------------------------------------------

        var question = await (
            from q in _context.Set<AssessmentQuestion>()

            join sessionRoundLocal in _context
                .Set<AssessmentSessionRound>()
                on q.AssessmentSessionRoundId
                    equals sessionRoundLocal.Id

            join session in _context
                .Set<AssessmentSession>()
                on sessionRoundLocal.AssessmentSessionId
                    equals session.Id

            join candidateApplication in _context
                .Set<CandidateApplication>()
                on session.CandidateApplicationId
                    equals candidateApplication.Id

            where q.Id == request.AssessmentQuestionId
                  && candidateApplication.OrganizationId
                      == organizationId

            select q
        ).FirstOrDefaultAsync(
            cancellationToken);

        if (question is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.AssessmentQuestionId}' was not found."));
        }

        // ---------------------------------------------------------
        // 3. Validate question status
        // ---------------------------------------------------------

        if (question.Status !=
            AssessmentQuestionStatus.InProgress)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.NotInProgress",
                    "Candidate code can only be recorded for an in-progress assessment question."));
        }

        // ---------------------------------------------------------
        // 4. Load session round
        //
        // Scope it through the question's owning session round.
        // ---------------------------------------------------------

        var sessionRound = await _context
            .Set<AssessmentSessionRound>()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == question.AssessmentSessionRoundId,
                cancellationToken);

        if (sessionRound is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "AssessmentSessionRound.NotFound",
                    $"Assessment session round '{question.AssessmentSessionRoundId}' was not found."));
        }

        // ---------------------------------------------------------
        // 5. Validate round type
        //
        // Code can only be submitted for Coding / DSA rounds.
        // ---------------------------------------------------------

        if (sessionRound.Type is not AssessmentRoundType.Coding
            and not AssessmentRoundType.Dsa)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.CodeNotAllowed",
                    "Candidate code can only be recorded for Coding or DSA assessment rounds."));
        }

        // ---------------------------------------------------------
        // 6. Validate code
        // ---------------------------------------------------------

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.CodeRequired",
                    "Candidate code cannot be empty."));
        }

        // ---------------------------------------------------------
        // 7. Save candidate code
        // ---------------------------------------------------------

        question.SetCandidateCode(
            request.Code);

        await _context.SaveChangesAsync(
            cancellationToken);

        // ---------------------------------------------------------
        // 8. Do NOT generate follow-up questions.
        //
        // MVP decision:
        // Candidate can switch between the code editor and text
        // editor without triggering AI follow-up generation.
        // ---------------------------------------------------------

        return Result.Success();
    }
}