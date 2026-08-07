

using TAO.Domain.Enums;

namespace TAO.Application.HiringStrategies.Get;

public sealed record GetHiringStrategyResponse
{
    public Guid Id { get; init; }

    public Guid CampaignId { get; init; }

    public string GeneratedContent { get; init; } = string.Empty;

    public string StructuredContent { get; init; } = string.Empty;

    public HiringStrategyStatus Status { get; init; }

    public string ProviderName { get; init; } = string.Empty;

    public string ModelName { get; init; } = string.Empty;

    public int PromptVersion { get; init; }

    public DateTime CreatedOnUtc { get; init; }
}
