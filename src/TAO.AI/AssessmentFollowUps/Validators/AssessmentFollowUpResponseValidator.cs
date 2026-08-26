using TAO.AI.AssessmentFollowUps.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentFollowUps.Validators;

internal sealed class AssessmentFollowUpResponseValidator
{
    public Result Validate(
        AssessmentFollowUpAiResponse response)
    {
        if (response is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentFollowUp.InvalidResponse",
                    "The AI response is empty."));
        }

        if (string.IsNullOrWhiteSpace(response.Question))
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentFollowUp.QuestionRequired",
                    "The AI response must contain a follow-up question."));
        }

        return Result.Success();
    }
}