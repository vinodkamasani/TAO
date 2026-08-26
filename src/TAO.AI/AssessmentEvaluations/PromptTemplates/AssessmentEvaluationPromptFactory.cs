using System.Reflection;
using System.Text.Json;
using TAO.Domain.Entities;

namespace TAO.AI.AssessmentEvaluations.PromptTemplates;

internal sealed class AssessmentEvaluationPromptFactory
{
    private const string RoundEvaluationsToken =
        "{{RoundEvaluations}}";

    public async Task<string> CreateAsync(
        AssessmentSession session,
        IReadOnlyCollection<AssessmentRoundEvaluation> roundEvaluations,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(roundEvaluations);

        var template =
            await LoadTemplateAsync(cancellationToken);

        var evaluations =
            roundEvaluations.Select(
                x => new
                {
                    x.Score,
                    x.Confidence,
                    x.Strengths,
                    x.Gaps,
                    x.Evidence
                });

        var serializedEvaluations =
            JsonSerializer.Serialize(evaluations);

        return template
            .Replace(
                RoundEvaluationsToken,
                serializedEvaluations,
                StringComparison.Ordinal);
    }

    private static async Task<string> LoadTemplateAsync(
        CancellationToken cancellationToken)
    {
        var assembly =
            typeof(AssessmentEvaluationPromptFactory).Assembly;

        var resourceName =
            assembly.GetManifestResourceNames()
                .FirstOrDefault(
                    x => x.EndsWith(
                        "AssessmentEvaluationPrompt.md",
                        StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available =
                string.Join(
                    ", ",
                    assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"Prompt template 'AssessmentEvaluationPrompt.md' " +
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