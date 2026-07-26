using System.Text.Json;
using TAO.AI.Common;
using TAO.AI.JobProfiles.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.JobProfiles.Parsers;

internal sealed class JobProfileResponseParser 
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Result<JobProfileAiResponse> Parse(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result<JobProfileAiResponse>.Failure(
                    AiErrors.InvalidJsonResponse);
        }

        try
        {
            var aiResponse = JsonSerializer.Deserialize<JobProfileAiResponse>(
                content,
                JsonSerializerOptions);

            if (aiResponse is null)
            {
                return Result<JobProfileAiResponse>.Failure(
                    AiErrors.InvalidJsonResponse);
            }

            return Result<JobProfileAiResponse>.Success(aiResponse);
        }
        catch (JsonException)
        {
            return Result<JobProfileAiResponse>.Failure(
                AiErrors.InvalidJsonResponse);
        }
    }
}