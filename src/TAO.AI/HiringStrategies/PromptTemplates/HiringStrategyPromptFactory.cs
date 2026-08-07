using System.Reflection;
using TAO.Domain.Entities;

namespace TAO.AI.HiringStrategies.PromptTemplates;

internal sealed class HiringStrategyPromptFactory
{
    private const string StructuredJobProfileToken = "{{StructuredJobProfile}}";
    private const string GeneratedContentToken = "{{GeneratedContent}}";

    public async Task<string> CreateAsync(
        JobProfile jobProfile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jobProfile);

        var template = await LoadTemplateAsync(cancellationToken);

        return template.Replace(
            StructuredJobProfileToken,
            jobProfile.StructuredProfile.Value.Trim(),
            StringComparison.Ordinal)
            .Replace(
                GeneratedContentToken,
                jobProfile.GeneratedContent.Value.Trim(),
                StringComparison.Ordinal);
    }

    private static async Task<string> LoadTemplateAsync(
        CancellationToken cancellationToken)
    {
        // Use the assembly that contains this type to reliably find the embedded resource.
        var assembly = typeof(HiringStrategyPromptFactory).Assembly;

        // Be defensive: enumerate available resources and pick the matching one.
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith("HiringStrategyPrompt.md", StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available = string.Join(", ", assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"Prompt template 'HiringStrategyPrompt.md' not found. Available resources: {available}");
        }

        await using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("Prompt template not found.");

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync(cancellationToken);
    }
}