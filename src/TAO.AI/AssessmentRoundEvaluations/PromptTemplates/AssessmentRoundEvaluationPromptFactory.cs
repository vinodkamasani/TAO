using System.Reflection;
using System.Text.Json;
using TAO.AI.AssessmentRoundEvaluations.Contracts;
using TAO.Domain.Entities;

namespace TAO.AI.AssessmentRoundEvaluations.PromptTemplates;

internal sealed class AssessmentRoundEvaluationPromptFactory
{
    private const string RoundTypeToken =
        "{{RoundType}}";

    private const string DifficultyToken =
        "{{Difficulty}}";

    private const string QuestionEvaluationsToken =
        "{{QuestionEvaluations}}";

    public async Task<string> CreateAsync(
        AssessmentSessionRound sessionRound,
        IReadOnlyCollection<AssessmentQuestionEvaluation> evaluations,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sessionRound);
        ArgumentNullException.ThrowIfNull(evaluations);

        var template =
            await LoadTemplateAsync(cancellationToken);

        var questionEvaluations =
            evaluations.Select(
                x => new
                {
                    x.Score,
                    x.Confidence,
                    x.Strengths,
                    x.Gaps,
                    x.Evidence
                });

        var serializedEvaluations =
            JsonSerializer.Serialize(
                questionEvaluations);

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
                QuestionEvaluationsToken,
                serializedEvaluations,
                StringComparison.Ordinal);
    }

    private static async Task<string> LoadTemplateAsync(
        CancellationToken cancellationToken)
    {
        var assembly =
            typeof(AssessmentRoundEvaluationPromptFactory).Assembly;

        var resourceName =
            assembly.GetManifestResourceNames()
                .FirstOrDefault(
                    x => x.EndsWith(
                        "AssessmentRoundEvaluationPrompt.md",
                        StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available =
                string.Join(
                    ", ",
                    assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"Prompt template 'AssessmentRoundEvaluationPrompt.md' " +
                $"not found. Available resources: {available}");
        }

        await using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                "Prompt template not found.");

        using var reader =
            new StreamReader(stream);

        return await reader.ReadToEndAsync(
            cancellationToken);
    }
}