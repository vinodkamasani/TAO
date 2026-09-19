using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Application.AssessmentStrategies.Get;

public sealed record GetAssessmentStrategyResponse(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    string AssessmentName,
    MarkdownContent Content,
    StructuredContent StructuredContent,
    string Status,
    DateTime GeneratedOn,
    Guid? ApprovedByUserId,
    DateTime? ApprovedOn);