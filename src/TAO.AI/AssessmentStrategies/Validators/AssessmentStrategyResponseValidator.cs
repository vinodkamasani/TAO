using TAO.AI.AssessmentStrategies.Contracts;
using TAO.AI.Common;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentStrategies.Validators;

internal sealed class AssessmentStrategyResponseValidator
{
    public Result Validate(AssessmentStrategyAiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (string.IsNullOrWhiteSpace(response.AssessmentName))
        {
            return Result.Failure(
                AiErrors.InvalidAssessmentStrategy);
        }

        if (response.Rounds is null ||
            response.Rounds.Count == 0)
        {
            return Result.Failure(
                AiErrors.InvalidAssessmentStrategy);
        }

        foreach (var round in response.Rounds)
        {
            if (round.Order <= 0 ||
                string.IsNullOrWhiteSpace(round.Type) ||
                string.IsNullOrWhiteSpace(round.Difficulty) ||
                round.DurationInMinutes <= 0 ||
                round.QuestionCount <= 0 ||
                round.Competencies is null)
            {
                return Result.Failure(
                    AiErrors.InvalidAssessmentStrategy);
            }

            foreach (var competency in round.Competencies)
            {
                if (string.IsNullOrWhiteSpace(competency.Name) ||
                    string.IsNullOrWhiteSpace(competency.Priority))
                {
                    return Result.Failure(
                        AiErrors.InvalidAssessmentStrategy);
                }
            }
        }

        return Result.Success();
    }
}