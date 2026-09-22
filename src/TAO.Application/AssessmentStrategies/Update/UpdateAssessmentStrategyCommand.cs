using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Update;

public sealed record UpdateAssessmentStrategyCommand(
    Guid AssessmentStrategyId,
    string AssessmentName,
    IReadOnlyCollection<UpdateAssessmentRoundCommand> Rounds)
    : IRequest<Result<UpdateAssessmentStrategyResponse>>;

public sealed record UpdateAssessmentRoundCommand(
    int Order,
    string Type,
    string Difficulty,
    int DurationInMinutes,
    int QuestionCount,
    IReadOnlyCollection<UpdateAssessmentCompetencyCommand> Competencies);

public sealed record UpdateAssessmentCompetencyCommand(
    string Name,
    string Priority,
    byte MinimumPassPercentage);