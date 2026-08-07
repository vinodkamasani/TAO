using System.Reflection;
using TAO.AI.ResumeScreening.Contracts;

namespace TAO.AI.ResumeScreening.PromptTemplates;

internal sealed class ResumeScreeningPromptFactory
{
    private const string JobProfileToken = "{{JobProfile}}";
    private const string HiringStrategyToken = "{{HiringStrategy}}";
    private const string ResumeProfileToken = "{{ResumeProfile}}";

    public async Task<string> CreateAsync(
        ResumeScreeningRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var template = await LoadTemplateAsync(
            cancellationToken);

        return template
            .Replace(
                JobProfileToken,
                request.JobProfile.Trim(),
                StringComparison.Ordinal)
            .Replace(
                HiringStrategyToken,
                request.HiringStrategy.Trim(),
                StringComparison.Ordinal)
            .Replace(
                ResumeProfileToken,
                request.ResumeProfile.Trim(),
                StringComparison.Ordinal);

    }

    private static async Task<string> LoadTemplateAsync(
        CancellationToken cancellationToken)
    {
        // Use the assembly that contains this type to reliably find the embedded resource.
        var assembly = typeof(ResumeScreeningPromptFactory).Assembly;

        // Be defensive: enumerate available resources and pick the matching one.
        var resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(x =>
                x.EndsWith(
                    "ResumeScreeningPrompt.md",
                    StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available = string.Join(
                ", ",
                assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"Prompt template 'ResumeScreeningPrompt.md' not found. Available resources: {available}");
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