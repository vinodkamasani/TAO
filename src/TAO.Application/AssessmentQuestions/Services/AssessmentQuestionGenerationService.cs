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
    private const int MaxDsaGenerationAttempts = 3;
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
         * DSA history is used only for DSA question generation.
         *
         * We maintain two representations:
         *
         * 1. usedQuestionStarts:
         *    Small representation sent to the LLM to discourage repetition.
         *
         * 2. existingQuestionHashes:
         *    Full normalized questions used by the application as the
         *    final duplicate protection.
         */
        var usedQuestionStarts = new List<string>();

      

        var existingQuestionHashes = new HashSet<string>(
            StringComparer.Ordinal);

        if (sessionRound.Type == AssessmentRoundType.Dsa)
        {
            var dsaRoundIds = await _context
                .Set<AssessmentSessionRound>()
                .Where(x =>
                    x.AssessmentSessionId == session.Id &&
                    x.Type == AssessmentRoundType.Dsa)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var existingDsaQuestions = await _context
                .Set<AssessmentQuestion>()
                .Where(x =>
                    dsaRoundIds.Contains(
                        x.AssessmentSessionRoundId))
                .OrderBy(x => x.Order)
                .ToListAsync(cancellationToken);

            foreach (var existingQuestion in existingDsaQuestions)
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
        }

        for (var attempt = 1;
             attempt <= MaxDsaGenerationAttempts;
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

            /*
             * Duplicate protection is required only for DSA.
             *
             * Other round types continue to use their existing
             * generation behavior.
             */
            if (sessionRound.Type == AssessmentRoundType.Dsa)
            {
                var normalizedGeneratedQuestion =
                    NormalizeQuestion(
                        generatedQuestion.Question);

                if (existingQuestionHashes.Contains(
                        normalizedGeneratedQuestion))
                {
                    /*
                     * The LLM generated an existing question.
                     *
                     * Do not return a validation error.
                     * Retry generation instead.
                     *
                     * Add the generated question start to the prompt
                     * history in case the model produced something that
                     * wasn't already present in the history.
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
                "Unable to generate a new DSA question that has not already been used in this assessment session."));
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