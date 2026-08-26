using TAO.AI.AssessmentRoundEvaluations.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentRoundEvaluations.Validators;

internal sealed class AssessmentRoundEvaluationResponseValidator
{
    public Result Validate(
        AssessmentRoundEvaluationAiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.Confidence > 100)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentRoundEvaluation.InvalidConfidence",
                    "Confidence must be between 0 and 100."));
        }

        if (response.Strengths is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentRoundEvaluation.InvalidStrengths",
                    "Strengths must be provided."));
        }

        if (response.Gaps is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentRoundEvaluation.InvalidGaps",
                    "Gaps must be provided."));
        }

        if (response.Evidence is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentRoundEvaluation.InvalidEvidence",
                    "Evidence must be provided."));
        }

        return Result.Success();
    }
}