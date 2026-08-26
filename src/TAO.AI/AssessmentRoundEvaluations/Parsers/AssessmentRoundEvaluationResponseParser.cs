using System.Text.Json;
using TAO.AI.AssessmentRoundEvaluations.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentRoundEvaluations.Parsers;

internal sealed class AssessmentRoundEvaluationResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Result<AssessmentRoundEvaluationAiResponse> Parse(
        string rawResponse)
    {

        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return Result<AssessmentRoundEvaluationAiResponse>.Failure(
                Error.Validation(
                    "AssessmentRoundEvaluation.EmptyResponse",
                    "The AI round evaluation response was empty."));
        }

        try
        {
            var response =
                JsonSerializer.Deserialize<
                    AssessmentRoundEvaluationAiResponse>(
                    rawResponse,
                    JsonOptions);

            if (response is null)
            {
                return Result<AssessmentRoundEvaluationAiResponse>.Failure(
                    Error.Validation(
                        "AssessmentRoundEvaluation.InvalidResponse",
                        "The AI round evaluation response could not be deserialized."));
            }

            return Result<AssessmentRoundEvaluationAiResponse>.Success(
                response);
        }
        catch (JsonException ex)
        {
            return Result<AssessmentRoundEvaluationAiResponse>.Failure(
                Error.Validation(
                    "AssessmentRoundEvaluation.InvalidJson",
                    $"The AI round evaluation response contains invalid JSON: {ex.Message}"));
        }
    }
}