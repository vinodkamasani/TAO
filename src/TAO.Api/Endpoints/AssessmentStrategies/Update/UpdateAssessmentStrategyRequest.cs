namespace TAO.Api.Endpoints.AssessmentStrategies.Update;

public sealed record UpdateAssessmentStrategyRequest(
    string AssessmentName,
    IReadOnlyCollection<UpdateAssessmentRoundRequest> Rounds);

public sealed record UpdateAssessmentRoundRequest(
    int Order,
    string Type,
    string Difficulty,
    int DurationInMinutes,
    int QuestionCount,
    IReadOnlyCollection<UpdateAssessmentCompetencyRequest> Competencies);

public sealed record UpdateAssessmentCompetencyRequest(
    string Name,
    string Priority,
    byte MinimumPassPercentage);