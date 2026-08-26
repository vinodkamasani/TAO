using System.Text.Json;
using TAO.AI.AssessmentQuestions.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentQuestions.Parsers;

internal sealed class AssessmentQuestionResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Result<AssessmentQuestionAiResponse> Parse(
        string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return Result<AssessmentQuestionAiResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.EmptyResponse",
                    "The AI returned an empty response."));
        }

        try
        {
            var response =
                JsonSerializer.Deserialize<AssessmentQuestionAiResponse>(
                    rawResponse,
                    JsonOptions);

            if (response is null)
            {
                return Result<AssessmentQuestionAiResponse>.Failure(
                    Error.Validation(
                        "AssessmentQuestion.InvalidResponse",
                        "The AI response could not be parsed."));
            }

            return Result<AssessmentQuestionAiResponse>.Success(
                response);
        }
        catch (JsonException ex)
        {
            return Result<AssessmentQuestionAiResponse>.Failure(
                Error.Validation(
                    "AssessmentQuestion.InvalidJson",
                    $"The AI response was not valid JSON: {ex.Message}"));
        }
    }
}