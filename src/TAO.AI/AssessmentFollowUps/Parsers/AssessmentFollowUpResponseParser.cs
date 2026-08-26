using System.Text.Json;
using TAO.AI.AssessmentFollowUps.Contracts;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentFollowUps.Parsers;

internal sealed class AssessmentFollowUpResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Result<AssessmentFollowUpAiResponse> Parse(
        string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return Result<AssessmentFollowUpAiResponse>.Failure(
                Error.Validation(
                    "AssessmentFollowUp.EmptyResponse",
                    "The AI returned an empty response."));
        }

        try
        {
            var response =
                JsonSerializer.Deserialize<AssessmentFollowUpAiResponse>(
                    rawResponse,
                    JsonOptions);

            if (response is null)
            {
                return Result<AssessmentFollowUpAiResponse>.Failure(
                    Error.Validation(
                        "AssessmentFollowUp.InvalidResponse",
                        "The AI response could not be parsed."));
            }

            return Result<AssessmentFollowUpAiResponse>.Success(
                response);
        }
        catch (JsonException ex)
        {
            return Result<AssessmentFollowUpAiResponse>.Failure(
                Error.Validation(
                    "AssessmentFollowUp.InvalidJson",
                    $"The AI response was not valid JSON: {ex.Message}"));
        }
    }
}