using TAO.Domain.ValueObjects;

namespace TAO.Application.AssessmentStrategies.Contracts;

public sealed record AssessmentStrategyResponse(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    string AssessmentName,
    MarkdownContent Content,
    StructuredContent StructuredContent,
    string Status,
    DateTime GeneratedOn,
    IReadOnlyCollection<AssessmentRoundResponse> Rounds);

public sealed record AssessmentRoundResponse(
    Guid Id,
    int Order,
    string Type,
    string Difficulty,
    int DurationInMinutes,
    int TargetQuestionCount,
    IReadOnlyCollection<AssessmentCompetencyResponse> Competencies);

public sealed record AssessmentCompetencyResponse(
    string Name,
    string Priority,
    int MinimumPassPercentage);