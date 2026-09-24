using TAO.Domain.ValueObjects;

namespace TAO.Application.HiringStrategies.Contracts;

public sealed record HiringStrategyResponse(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    MarkdownContent Content,
    StructuredContent StructuredContent,
    string Status,
    string ProviderName,
    string ModelName,
    int PromptVersion,
    DateTime CreatedOnUtc);