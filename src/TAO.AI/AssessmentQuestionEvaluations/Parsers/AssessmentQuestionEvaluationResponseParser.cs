using System.Text.Json;
using TAO.AI.AssessmentQuestionEvaluations.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentQuestionEvaluations.Parsers;

internal sealed class AssessmentQuestionEvaluationResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Result<AssessmentQuestionEvaluationAiResponse> Parse(
        string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return Result<AssessmentQuestionEvaluationAiResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.EmptyResponse",
                    "The AI evaluation response was empty."));
        }

        try
        {
            var response =
                JsonSerializer.Deserialize<
                    AssessmentQuestionEvaluationAiResponse>(
                    rawResponse,
                    JsonOptions);

            if (response is null)
            {
                return Result<AssessmentQuestionEvaluationAiResponse>.Failure(
                    Error.Validation(
                        "AssessmentQuestionEvaluation.InvalidResponse",
                        "The AI evaluation response could not be deserialized."));
            }

            return Result<AssessmentQuestionEvaluationAiResponse>.Success(
                response);
        }
        catch (JsonException ex)
        {
            return Result<AssessmentQuestionEvaluationAiResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestionEvaluation.InvalidJson",
                    $"The AI evaluation response contains invalid JSON: {ex.Message}"));
        }
    }
}