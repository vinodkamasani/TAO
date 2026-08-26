using System.Reflection;
using TAO.Domain.Entities;
using TAO.Domain.Enums;

namespace TAO.AI.AssessmentFollowUps.PromptTemplates;

internal sealed class AssessmentFollowUpPromptFactory
{
    private const string RoundTypeToken =
        "{{RoundType}}";

    private const string DifficultyToken =
        "{{Difficulty}}";

    private const string PrimaryQuestionToken =
        "{{PrimaryQuestion}}";

    private const string ConversationToken =
        "{{Conversation}}";

    private const string CodeToken =
     "{{Code}}";


    public async Task<string> CreateAsync(
        AssessmentQuestion question,
        AssessmentSessionRound sessionRound,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(sessionRound);

        if (question.Conversation is null)
        {
            throw new InvalidOperationException(
                "Assessment question conversation has not been initialized.");
        }

        var template =
            await LoadTemplateAsync(
                sessionRound.Type,
                cancellationToken);

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
                PrimaryQuestionToken,
                question.PrimaryQuestion.Trim(),
                StringComparison.Ordinal)
            .Replace(
                ConversationToken,
                question.Conversation.Value.Trim(),
                StringComparison.Ordinal)
             .Replace(
                CodeToken,
                question.CandidateCode?.Trim()?? string.Empty,
                StringComparison.Ordinal);
    }

    private static async Task<string> LoadTemplateAsync(
        AssessmentRoundType roundType,
        CancellationToken cancellationToken)
    {
        var templateName =
            GetTemplateName(roundType);

        var assembly =
            typeof(AssessmentFollowUpPromptFactory).Assembly;

        var resourceName =
            assembly.GetManifestResourceNames()
                .FirstOrDefault(
                    x => x.EndsWith(
                        templateName,
                        StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available =
                string.Join(
                    ", ",
                    assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"Prompt template '{templateName}' not found. " +
                $"Available resources: {available}");
        }

        await using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Prompt template '{templateName}' could not be loaded.");

        using var reader =
            new StreamReader(stream);

        return await reader.ReadToEndAsync(
            cancellationToken);
    }

    private static string GetTemplateName(
        AssessmentRoundType roundType)
    {
        return roundType switch
        {
            AssessmentRoundType.Dsa =>
                "AssessmentFollowUp_DSA.md",

            AssessmentRoundType.Coding =>
                "AssessmentFollowUp_Coding.md",

            AssessmentRoundType.TechnicalDiscussion =>
                "AssessmentFollowUp_TechnicalDiscussion.md",

            AssessmentRoundType.SystemDesign =>
                "AssessmentFollowUp_SystemDesign.md",

            AssessmentRoundType.AIRound =>
                "AssessmentFollowUp_AI.md",

            _ => throw new ArgumentOutOfRangeException(
                nameof(roundType),
                roundType,
                "Unsupported assessment round type.")
        };
    }
}