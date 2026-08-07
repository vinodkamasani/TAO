using TAO.Domain.Common;

namespace TAO.Domain.Common;

public abstract class AiGeneratedArtifact : Entity
{
    protected AiGeneratedArtifact()
    {
    }

    protected AiGeneratedArtifact(
        string prompt,
        string rawResponse,
        string providerName,
        string modelName,
        int promptVersion)
    {
        Prompt = Guard.AgainstNullOrWhiteSpace(prompt, nameof(prompt));
        RawResponse = Guard.AgainstNullOrWhiteSpace(rawResponse, nameof(rawResponse));
        ProviderName = Guard.AgainstNullOrWhiteSpace(providerName, nameof(providerName));
        ModelName = Guard.AgainstNullOrWhiteSpace(modelName, nameof(modelName));
        PromptVersion = Guard.AgainstGreaterThanZero(promptVersion, nameof(promptVersion));

        GeneratedOn = DateTime.UtcNow;
    }

    public string Prompt { get; protected set; } = null!;

    public string RawResponse { get; protected set; } = null!;

    public string ProviderName { get; protected set; } = null!;

    public string ModelName { get; protected set; } = null!;

    public int PromptVersion { get; protected set; }

    public DateTime GeneratedOn { get; protected set; }
}