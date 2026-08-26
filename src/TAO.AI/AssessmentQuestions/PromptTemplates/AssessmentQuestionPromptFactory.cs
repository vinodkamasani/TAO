using System.Reflection;
using TAO.Domain.Entities;
using TAO.Domain.Enums;

namespace TAO.AI.AssessmentQuestions.PromptTemplates;

internal sealed class AssessmentQuestionPromptFactory
{
    private const string StructuredJobProfileToken =
        "{{StructuredJobProfile}}";

    private const string RoundTypeToken =
        "{{RoundType}}";

    private const string DifficultyToken =
        "{{Difficulty}}";

    private const string CompetenciesToken =
        "{{Competencies}}";

    private const string DurationInMinutesToken =
        "{{DurationInMinutes}}";

    private const string UsedQuestionStarts =
        "{{UsedQuestionStarts}}";


    public async Task<string> CreateAsync(
        JobProfile jobProfile,
        AssessmentSessionRound sessionRound,
        IReadOnlyCollection<string>? usedQuestionStarts = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jobProfile);
        ArgumentNullException.ThrowIfNull(sessionRound);

        var templateFileName = GetPromptTemplate(
            sessionRound.Type);

        var template = await LoadTemplateAsync(
            templateFileName,
            cancellationToken);

        var competencies = string.Join(
            Environment.NewLine,
            sessionRound.Competencies.Select(
                x => $"- {x.Name} (Priority: {x.Priority})"));

        var previousQuestionStarts =
            usedQuestionStarts is null ||
            usedQuestionStarts.Count == 0
                ? "None"
                : string.Join(
                    Environment.NewLine,
                    usedQuestionStarts
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => $"- {x.Trim()}"));

     

        return template
            .Replace(
                StructuredJobProfileToken,
                jobProfile.StructuredProfile.Value.Trim(),
                StringComparison.Ordinal)
            .Replace(
                RoundTypeToken,
                sessionRound.Type.ToString(),
                StringComparison.Ordinal)
            .Replace(
                DifficultyToken,
                sessionRound.Difficulty.ToString(),
                StringComparison.Ordinal)
            .Replace(
                CompetenciesToken,
                competencies,
                StringComparison.Ordinal)
            .Replace(
                DurationInMinutesToken,
                sessionRound.DurationInMinutes.ToString(),
                StringComparison.Ordinal)
            .Replace(
                UsedQuestionStarts,
                previousQuestionStarts,
                StringComparison.Ordinal)
            ;
    }

    private static string GetPromptTemplate(
        AssessmentRoundType roundType)
    {
        return roundType switch
        {
            AssessmentRoundType.Dsa =>
                "DsaQuestionPrompt.md",

            AssessmentRoundType.Coding =>
                "CodingQuestionPrompt.md",

            AssessmentRoundType.TechnicalDiscussion =>
                "TechnicalDiscussionQuestionPrompt.md",

            AssessmentRoundType.SystemDesign =>
                "SystemDesignQuestionPrompt.md",

            AssessmentRoundType.AIRound =>
                "AIRoundQuestionPrompt.md",

            _ => throw new ArgumentOutOfRangeException(
                nameof(roundType),
                roundType,
                "No assessment question prompt is configured for this round type.")
        };
    }

    private static async Task<string> LoadTemplateAsync(
        string templateFileName,
        CancellationToken cancellationToken)
    {
        var assembly =
            typeof(AssessmentQuestionPromptFactory).Assembly;

        var resourceName =
            assembly.GetManifestResourceNames()
                .FirstOrDefault(
                    x => x.EndsWith(
                        templateFileName,
                        StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available =
                string.Join(
                    ", ",
                    assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"Prompt template '{templateFileName}' " +
                $"not found. Available resources: {available}");
        }

        await using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Prompt template '{templateFileName}' not found.");

        using var reader =
            new StreamReader(stream);

        return await reader.ReadToEndAsync(
            cancellationToken);
    }
}