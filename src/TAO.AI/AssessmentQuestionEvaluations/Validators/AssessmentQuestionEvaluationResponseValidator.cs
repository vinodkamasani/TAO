using TAO.AI.AssessmentQuestionEvaluations.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentQuestionEvaluations.Validators;

internal sealed class AssessmentQuestionEvaluationResponseValidator
{
    public Result Validate(
        AssessmentQuestionEvaluationAiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.Score > 100)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.InvalidScore",
                    "Evaluation score must be between 0 and 100."));
        }

        if (response.Confidence > 100)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.InvalidConfidence",
                    "Evaluation confidence must be between 0 and 100."));
        }

        if (response.Strengths is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.InvalidStrengths",
                    "Strengths must be provided."));
        }

        if (response.Gaps is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.InvalidGaps",
                    "Gaps must be provided."));
        }

        if (response.Evidence is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.InvalidEvidence",
                    "Evidence must be provided."));
        }

        if (response.Competencies is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.InvalidCompetencies",
                    "Competencies must be provided."));
        }

        var competencyNames = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var competency in response.Competencies)
        {
            if (competency is null ||
                string.IsNullOrWhiteSpace(competency.Name))
            {
                return Result.Failure(
                    Error.Validation(
                        "AssessmentQuestionEvaluation.InvalidCompetency",
                        "Each competency must have a name."));
            }

            if (competency.Score > 100)
            {
                return Result.Failure(
                    Error.Validation(
                        "AssessmentQuestionEvaluation.InvalidCompetencyScore",
                        "Competency score must be between 0 and 100."));
            }

            if (!competencyNames.Add(
                    competency.Name.Trim()))
            {
                return Result.Failure(
                    Error.Validation(
                        "AssessmentQuestionEvaluation.DuplicateCompetency",
                        $"Competency '{competency.Name}' was provided more than once."));
            }
        }

        return Result.Success();
    }
}