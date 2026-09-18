using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Application.AssessmentStrategies.Create;

public sealed record CreateAssessmentStrategyResponse(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    string AssessmentName,
    MarkdownContent Content,
    StructuredContent StructuredContent,
    AssessmentStrategyStatus Status,
    DateTime GeneratedOn,
    IReadOnlyCollection<CreateAssessmentRoundResponse> Rounds);

public sealed record CreateAssessmentRoundResponse(
    Guid Id,
    int Order,
    string Type,
    string Difficulty,
    int DurationInMinutes,
    int TargetQuestionCount,
    IReadOnlyCollection<CreateAssessmentRoundCompetencyResponse> Competencies);

public sealed record CreateAssessmentRoundCompetencyResponse(
    string Name,
    string Priority,
    int MinimumPassPercentage);