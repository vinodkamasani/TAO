using TAO.AI.AssessmentEvaluations.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentEvaluations.Validators;

internal sealed class AssessmentEvaluationResponseValidator
{
    public Result Validate(
        AssessmentEvaluationAiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.Confidence > 100)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentEvaluation.InvalidConfidence",
                    "Confidence must be between 0 and 100."));
        }

        if (string.IsNullOrWhiteSpace(
                response.ExecutiveSummary))
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentEvaluation.ExecutiveSummaryRequired",
                    "Executive summary must be provided."));
        }

        if (response.Strengths is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentEvaluation.InvalidStrengths",
                    "Strengths must be provided."));
        }

        if (response.Gaps is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentEvaluation.InvalidGaps",
                    "Gaps must be provided."));
        }

        if (response.Evidence is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentEvaluation.InvalidEvidence",
                    "Evidence must be provided."));
        }

        return Result.Success();
    }
}