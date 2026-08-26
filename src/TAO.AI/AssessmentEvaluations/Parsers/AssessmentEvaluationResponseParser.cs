using System.Text.Json;
using TAO.AI.AssessmentEvaluations.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentEvaluations.Parsers;

internal sealed class AssessmentEvaluationResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Result<AssessmentEvaluationAiResponse> Parse(
        string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return Result<AssessmentEvaluationAiResponse>.Failure(
                Error.Validation(
                    "AssessmentEvaluation.EmptyResponse",
                    "The AI assessment evaluation response was empty."));
        }

        try
        {
            var response =
                JsonSerializer.Deserialize<
                    AssessmentEvaluationAiResponse>(
                    rawResponse,
                    JsonOptions);

            if (response is null)
            {
                return Result<AssessmentEvaluationAiResponse>.Failure(
                    Error.Validation(
                        "AssessmentEvaluation.InvalidResponse",
                        "The AI assessment evaluation response could not be deserialized."));
            }

            return Result<AssessmentEvaluationAiResponse>.Success(
                response);
        }
        catch (JsonException ex)
        {
            return Result<AssessmentEvaluationAiResponse>.Failure(
                Error.Validation(
                    "AssessmentEvaluation.InvalidJson",
                    $"The AI assessment evaluation response contains invalid JSON: {ex.Message}"));
        }
    }
}