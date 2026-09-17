using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Application.HiringStrategies.Create;

public sealed record CreateHiringStrategyResponse(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    MarkdownContent Content,
    StructuredContent StructuredContent,
    HiringStrategyStatus Status);