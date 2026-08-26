using System.Reflection;
using TAO.Domain.Entities;

namespace TAO.AI.AssessmentQuestionEvaluations.PromptTemplates;

internal sealed class AssessmentQuestionEvaluationPromptFactory
{
    private const string PromptFileName =
        "AssessmentQuestionEvaluationPrompt.md";

    private const string RoundTypeToken =
        "{{RoundType}}";

    private const string DifficultyToken =
        "{{Difficulty}}";

    private const string QuestionToken =
        "{{Question}}";

    private const string CompetenciesToken =
        "{{Competencies}}";

    private const string CandidateResponseToken =
        "{{CandidateResponse}}";

    private const string CandidateCodeToken =
        "{{CandidateCode}}";

    public async Task<string> CreateAsync(
        AssessmentQuestion question,
        AssessmentSessionRound sessionRound,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(sessionRound);

        var template = await LoadTemplateAsync(
            cancellationToken);

        var competencies = string.Join(
            Environment.NewLine,
            question.Competencies.Select(
                x => $"- {x}"));

        var candidateResponse =
            GetCandidateResponse(question);

        var candidateCode =
            string.IsNullOrWhiteSpace(question.CandidateCode)
                ? "Not provided."
                : question.CandidateCode;

        return template
            .Replace(
                RoundTypeToken,
                sessionRound.Type.ToString(),
                StringComparison.Ordinal)
            .Replace(
                DifficultyToken,
                sessionRound.Difficulty.ToString(),
                StringComparison.Ordinal)
            .Replace(
                QuestionToken,
                question.PrimaryQuestion,
                StringComparison.Ordinal)
            .Replace(
                CompetenciesToken,
                competencies,
                StringComparison.Ordinal)
            .Replace(
                CandidateResponseToken,
                candidateResponse,
                StringComparison.Ordinal)
            .Replace(
                CandidateCodeToken,
                candidateCode,
                StringComparison.Ordinal);
    }

    private static string GetCandidateResponse(
        AssessmentQuestion question)
    {
        if (question.Conversation is null ||
            string.IsNullOrWhiteSpace(
                question.Conversation.Value))
        {
            return "Not provided.";
        }

        return question.Conversation.Value;
    }

    private static async Task<string> LoadTemplateAsync(
        CancellationToken cancellationToken)
    {
        var assembly =
            typeof(AssessmentQuestionEvaluationPromptFactory)
                .Assembly;

        var resourceName =
            assembly
                .GetManifestResourceNames()
                .FirstOrDefault(
                    x => x.EndsWith(
                        PromptFileName,
                        StringComparison.Ordinal));

        if (resourceName is null)
        {
            throw new InvalidOperationException(
                $"Prompt template '{PromptFileName}' was not found.");
        }

        await using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Prompt template '{PromptFileName}' could not be loaded.");

        using var reader =
            new StreamReader(stream);

        return await reader.ReadToEndAsync(
            cancellationToken);
    }
}