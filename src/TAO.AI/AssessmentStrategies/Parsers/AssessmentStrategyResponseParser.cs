using System.Text.Json;
using TAO.AI.AssessmentStrategies.Contracts;
using TAO.AI.Common;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentStrategies.Parsers;

internal sealed class AssessmentStrategyResponseParser
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Result<AssessmentStrategyAiResponse> Parse(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result<AssessmentStrategyAiResponse>.Failure(
                AiErrors.InvalidJsonResponse);
        }

        try
        {
            var aiResponse =
                JsonSerializer.Deserialize<AssessmentStrategyAiResponse>(
                    content,
                    JsonSerializerOptions);

            if (aiResponse is null)
            {
                return Result<AssessmentStrategyAiResponse>.Failure(
                    AiErrors.InvalidJsonResponse);
            }

            return Result<AssessmentStrategyAiResponse>.Success(
                aiResponse);
        }
        catch (JsonException)
        {
            return Result<AssessmentStrategyAiResponse>.Failure(
                AiErrors.InvalidJsonResponse);
        }
    }
}