using System.Reflection;

namespace TAO.AI.ResumeParsing.PromptTemplates;

internal sealed class ResumePromptFactory
{
    private const string ResumeContentToken = "{{ResumeContent}}";

    public async Task<string> CreateAsync(
        string resumeContent,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resumeContent);

        var template = await LoadTemplateAsync(cancellationToken);

        return template.Replace(
            ResumeContentToken,
            resumeContent.Trim(),
            StringComparison.Ordinal);
    }

    private static async Task<string> LoadTemplateAsync(
        CancellationToken cancellationToken)
    {
        // Use the assembly that contains this type to reliably find the embedded resource.
        var assembly = typeof(ResumePromptFactory).Assembly;

        // Be defensive: enumerate available resources and pick the matching one.
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(x =>
                x.EndsWith("ResumeParsingPrompt.md", StringComparison.Ordinal));

        if (resourceName is null)
        {
            var available = string.Join(", ", assembly.GetManifestResourceNames());

            throw new InvalidOperationException(
                $"Prompt template 'ResumeParsingPrompt.md' not found. Available resources: {available}");
        }

        await using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("Prompt template not found.");

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync(cancellationToken);
    }
}