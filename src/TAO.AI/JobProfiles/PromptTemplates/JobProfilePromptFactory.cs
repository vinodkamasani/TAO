using System.Reflection;

namespace TAO.AI.JobProfiles.PromptTemplates;

internal sealed class JobProfilePromptFactory 
{
    private const string JobDescriptionToken = "{{JobDescription}}";

    public async Task<string> CreateAsync(
        string jobDescription,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jobDescription);

        var template = await LoadTemplateAsync(cancellationToken);

        return template.Replace(
            JobDescriptionToken,
            jobDescription.Trim(),
            StringComparison.Ordinal);
    }

    private static async Task<string> LoadTemplateAsync(
        CancellationToken cancellationToken)
    {
        // Use the assembly that contains this type to reliably find the embedded resource.
        var assembly = typeof(JobProfilePromptFactory).Assembly;

        // Be defensive: enumerate available resources and pick the matching one.
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith("JobProfilePrompt.md", StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available = string.Join(", ", assembly.GetManifestResourceNames());
            throw new InvalidOperationException($"Prompt template 'JobProfilePrompt.md' not found. Available resources: {available}");
        }

        await using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("Prompt template not found.");

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync(cancellationToken);
    }
}