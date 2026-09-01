using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TAO.AI.Abstractions;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Services;

internal sealed class AssessmentQuestionGenerationService
    : IAssessmentQuestionGenerationService
{
    private const int MaxQuestionGenerationAttempts = 3;
    private const int QuestionStartWordCount = 10;

    private readonly IApplicationDbContext _context;
    private readonly IAssessmentQuestionGenerator _questionGenerator;

    public AssessmentQuestionGenerationService(
        IApplicationDbContext context,
        IAssessmentQuestionGenerator questionGenerator)
    {
        _context = context;
        _questionGenerator = questionGenerator;
    }

    public async Task<Result<AssessmentQuestion>> GenerateNextAsync(
        AssessmentSession session,
        AssessmentSessionRound sessionRound,
        CancellationToken cancellationToken)
    {
        if (session.Status != AssessmentSessionStatus.InProgress)
        {
            return Result<AssessmentQuestion>.Failure(
                Error.Validation(
                    "AssessmentSession.NotInProgress",
                    "An assessment question can only be generated for an in-progress assessment session."));
        }

        if (sessionRound.Status != AssessmentSessionRoundStatus.InProgress)
        {
            return Result<AssessmentQuestion>.Failure(
                Error.Validation(
                    "AssessmentSessionRound.NotInProgress",
                    "A question can only be generated for an in-progress assessment round."));
        }

        var lastQuestionOrder = await _context
            .Set<AssessmentQuestion>()
            .Where(x =>
                x.AssessmentSessionRoundId == sessionRound.Id)
            .Select(x => (int?)x.Order)
            .MaxAsync(cancellationToken);

        var nextOrder = (lastQuestionOrder ?? 0) + 1;

        if (nextOrder > sessionRound.TargetQuestionCount)
        {
            return Result<AssessmentQuestion>.Failure(
                Error.Validation(
                    "AssessmentSessionRound.QuestionLimitReached",
                    "The target number of primary questions for the current assessment round has been reached."));
        }

        var candidateApplication = await _context
            .Set<CandidateApplication>()
            .FirstOrDefaultAsync(
                x => x.Id == session.CandidateApplicationId,
                cancellationToken);

        if (candidateApplication is null)
        {
            return Result<AssessmentQuestion>.Failure(
                Error.NotFound(
                    "CandidateApplication.NotFound",
                    $"Candidate application '{session.CandidateApplicationId}' was not found."));
        }

        var jobProfile = await _context
            .Set<JobProfile>()
            .FirstOrDefaultAsync(
                x => x.CampaignId == candidateApplication.CampaignId,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result<AssessmentQuestion>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"No Job Profile was found for Campaign '{candidateApplication.CampaignId}'."));
        }

        /*
         * Keep question history for every round type.
         *
         * 1. usedQuestionStarts:
         *    Sent to the LLM to discourage generating a previously used
         *    question.
         *
         * 2. existingQuestionHashes:
         *    Application-side duplicate protection. This is the final
         *    check before accepting a generated question.
         */
        var usedQuestionStarts = new List<string>();

        var existingQuestionHashes = new HashSet<string>(
            StringComparer.Ordinal);

        var existingQuestions = await _context
            .Set<AssessmentQuestion>()
            .Where(x =>
                x.AssessmentSessionRoundId == sessionRound.Id)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        foreach (var existingQuestion in existingQuestions)
        {
            var normalizedQuestion =
                NormalizeQuestion(
                    existingQuestion.PrimaryQuestion);

            if (!string.IsNullOrWhiteSpace(
                    normalizedQuestion))
            {
                existingQuestionHashes.Add(
                    normalizedQuestion);
            }

            var questionStart =
                GetQuestionStart(
                    existingQuestion.PrimaryQuestion);

            if (!string.IsNullOrWhiteSpace(
                    questionStart))
            {
                usedQuestionStarts.Add(
                    questionStart);
            }
        }

        for (var attempt = 1;
             attempt <= MaxQuestionGenerationAttempts;
             attempt++)
        {
            var aiResult = await _questionGenerator.GenerateAsync(
                jobProfile,
                sessionRound,
                usedQuestionStarts,
                cancellationToken);

            if (aiResult.IsFailure)
            {
                return Result<AssessmentQuestion>.Failure(
                    aiResult.Error!);
            }

            var generatedQuestion =
                aiResult.Value!.Response;

            var normalizedGeneratedQuestion =
                NormalizeQuestion(
                    generatedQuestion.Question);

            if (existingQuestionHashes.Contains(
                    normalizedGeneratedQuestion))
            {
                /*
                 * The LLM generated a question that already exists.
                 *
                 * Retry generation rather than returning the duplicate.
                 */
                var duplicateQuestionStart =
                    GetQuestionStart(
                        generatedQuestion.Question);

                if (!string.IsNullOrWhiteSpace(
                        duplicateQuestionStart) &&
                    !usedQuestionStarts.Contains(
                        duplicateQuestionStart,
                        StringComparer.OrdinalIgnoreCase))
                {
                    usedQuestionStarts.Add(
                        duplicateQuestionStart);
                }

                continue;
            }

            var assessmentQuestion = AssessmentQuestion.Create(
                sessionRound.Id,
                nextOrder,
                generatedQuestion.Question,
                generatedQuestion.Competencies);

            var startedOn = DateTime.UtcNow;

            assessmentQuestion.Start(
                startedOn);

            var conversation = JsonSerializer.Serialize(
                new[]
                {
                    new
                    {
                        role = "assistant",
                        content = generatedQuestion.Question
                    }
                });

            assessmentQuestion.UpdateConversation(
                ConversationContent.Create(
                    conversation));

            return Result<AssessmentQuestion>.Success(
                assessmentQuestion);
        }

        return Result<AssessmentQuestion>.Failure(
            Error.Validation(
                "AssessmentQuestion.GenerationFailed",
                "Unable to generate a new question that has not already been used in this assessment round."));
    }

    private static string GetQuestionStart(
        string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return string.Empty;
        }

        var words = question
            .Trim()
            .Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries);

        return string.Join(
            " ",
            words.Take(QuestionStartWordCount));
    }

    private static string NormalizeQuestion(
        string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return string.Empty;
        }

        return string.Join(
            " ",
            question
                .Trim()
                .ToLowerInvariant()
                .Split(
                    (char[]?)null,
                    StringSplitOptions.RemoveEmptyEntries))
            .Trim(
                '.',
                ',',
                ':',
                ';',
                '!',
                '?',
                '-',
                '_');
    }
}