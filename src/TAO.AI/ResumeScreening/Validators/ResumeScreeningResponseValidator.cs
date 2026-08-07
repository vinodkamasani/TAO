using System.Text.Json;
using TAO.AI.ResumeScreening.Models;
using TAO.SharedKernel.Results;

namespace TAO.AI.ResumeScreening.Validators;

internal sealed class ResumeScreeningResponseValidator
{
    public Result Validate(
        ResumeScreeningResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        // ------------------------------------------------------------
        // Overall Match Percentage
        // ------------------------------------------------------------

        if (response.OverallMatchPercentage > 100)
        {
            return Result.Failure(
                Error.Validation(
                    "ResumeScreening.InvalidMatchPercentage",
                    "Overall Match Percentage must be between 0 and 100."));
        }


        // ------------------------------------------------------------
        // Structured Content
        // ------------------------------------------------------------

        if (response.StructuredContent.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure(
                Error.Validation(
                    "ResumeScreening.InvalidStructuredContent",
                    "Structured content must be a valid JSON object."));
        }

        return Result.Success();
    }
}