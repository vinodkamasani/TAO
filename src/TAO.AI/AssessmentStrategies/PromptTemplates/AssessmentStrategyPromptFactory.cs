using System.Reflection;

namespace TAO.AI.AssessmentStrategies.PromptTemplates;

internal sealed class AssessmentStrategyPromptFactory
{
    private const string JobProfileToken = "{{JobProfile}}";
    private const string HiringStrategyToken = "{{HiringStrategy}}";

    public async Task<string> CreateAsync(
        string jobProfile,
        string hiringStrategy,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jobProfile);
        ArgumentException.ThrowIfNullOrWhiteSpace(hiringStrategy);

        var template = await LoadTemplateAsync(cancellationToken);

        return template
            .Replace(
                JobProfileToken,
                jobProfile.Trim(),
                StringComparison.Ordinal)
            .Replace(
                HiringStrategyToken,
                hiringStrategy.Trim(),
                StringComparison.Ordinal);
    }

    private static async Task<string> LoadTemplateAsync(
        CancellationToken cancellationToken)
    {
        var assembly =
            typeof(AssessmentStrategyPromptFactory).Assembly;

        var resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(
                x => x.EndsWith(
                    "AssessmentStrategyPrompt.md",
                    StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available = string.Join(
                ", ",
                assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"Prompt template 'AssessmentStrategyPrompt.md' not found. " +
                $"Available resources: {available}");
        }

        await using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                "Prompt template not found.");

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync(
            cancellationToken);
    }
}