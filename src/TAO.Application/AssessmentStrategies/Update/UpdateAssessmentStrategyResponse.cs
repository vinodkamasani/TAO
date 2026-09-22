using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Application.AssessmentStrategies.Update;

public sealed record UpdateAssessmentStrategyResponse(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    string AssessmentName,
    MarkdownContent Content,
    StructuredContent StructuredContent,
    AssessmentStrategyStatus Status,
    DateTime GeneratedOn,
    IReadOnlyCollection<UpdateAssessmentRoundResponse> Rounds);

public sealed record UpdateAssessmentRoundResponse(
    Guid Id,
    int Order,
    string Type,
    string Difficulty,
    int DurationInMinutes,
    int TargetQuestionCount,
    IReadOnlyCollection<UpdateAssessmentCompetencyResponse> Competencies);

public sealed record UpdateAssessmentCompetencyResponse(
    string Name,
    string Priority,
    byte MinimumPassPercentage);