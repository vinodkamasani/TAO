using System.Text.Json;
using TAO.AI.Common;
using TAO.AI.HiringStrategies.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.HiringStrategies.Parsers;

internal sealed class HiringStrategyResponseParser
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Result<HiringStrategyAiResponse> Parse(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result<HiringStrategyAiResponse>.Failure(
                AiErrors.InvalidJsonResponse);
        }

        try
        {
            var aiResponse = JsonSerializer.Deserialize<HiringStrategyAiResponse>(
                content,
                JsonSerializerOptions);

            if (aiResponse is null)
            {
                return Result<HiringStrategyAiResponse>.Failure(
                    AiErrors.InvalidJsonResponse);
            }

            return Result<HiringStrategyAiResponse>.Success(aiResponse);
        }
        catch (JsonException)
        {
            return Result<HiringStrategyAiResponse>.Failure(
                AiErrors.InvalidJsonResponse);
        }
    }
}