using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentQuestionCode;

internal sealed class GetAssessmentQuestionCodeQueryHandler
    : IRequestHandler<
        GetAssessmentQuestionCodeQuery,
        Result<GetAssessmentQuestionCodeResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetAssessmentQuestionCodeQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<GetAssessmentQuestionCodeResponse>> Handle(
        GetAssessmentQuestionCodeQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result<GetAssessmentQuestionCodeResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResults.Unauthorized",
                    "Authentication is required."));
        }

        if (!_currentUser.UserId.HasValue)
        {
            return Result<GetAssessmentQuestionCodeResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResults.UserNotFound",
                    "The authenticated user could not be resolved."));
        }

        if (!_currentUser.OrganizationId.HasValue)
        {
            return Result<GetAssessmentQuestionCodeResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResults.OrganizationNotFound",
                    "The authenticated user's organization could not be resolved."));
        }

        if (_currentUser.Role is not UserRole.Recruiter
            and not UserRole.HiringManager)
        {
            return Result<GetAssessmentQuestionCodeResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentResults.Forbidden",
                    "Only recruiters and hiring managers can view assessment results."));
        }

        var organizationId = _currentUser.OrganizationId.Value;

        var questionData = await (
            from assessmentQuestion in _context
                .Set<AssessmentQuestion>()

            join sessionRound in _context
                .Set<AssessmentSessionRound>()
                on assessmentQuestion.AssessmentSessionRoundId
                    equals sessionRound.Id

            join assessmentSession in _context
                .Set<AssessmentSession>()
                on sessionRound.AssessmentSessionId
                    equals assessmentSession.Id

            join candidateApplication in _context
                .Set<CandidateApplication>()
                on assessmentSession.CandidateApplicationId
                    equals candidateApplication.Id

            where assessmentQuestion.Id == request.QuestionId
                  && assessmentSession.Id == request.AssessmentSessionId
                  && candidateApplication.OrganizationId == organizationId

            select new
            {
                Question = assessmentQuestion,
                RoundId = sessionRound.Id
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (questionData is null)
        {
            return Result<GetAssessmentQuestionCodeResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.NotFound",
                    $"Assessment question '{request.QuestionId}' was not found."));
        }

        // Replace CandidateCode with the actual property
        // from AssessmentQuestion.
        var code = questionData.Question.CandidateCode;

        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<GetAssessmentQuestionCodeResponse>.Failure(
                Error.NotFound(
                    "AssessmentQuestion.CodeNotFound",
                    "No candidate code is available for this assessment question."));
        }

        return Result<GetAssessmentQuestionCodeResponse>.Success(
            new GetAssessmentQuestionCodeResponse(
                request.AssessmentSessionId,
                questionData.RoundId,
                request.QuestionId,
                code));
    }
}