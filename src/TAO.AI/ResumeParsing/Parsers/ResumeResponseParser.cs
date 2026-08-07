using System.Text.Json;
using TAO.AI.ResumeParsing.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.ResumeParsing.Parsers;

internal sealed class ResumeResponseParser
{
    public Result<ResumeParsingResponse> Parse(
        string llmResponse)
    {
        if (string.IsNullOrWhiteSpace(llmResponse))
        {
            return Result<ResumeParsingResponse>.Failure(
                Error.Failure(
                    "ResumeParsing.EmptyResponse",
                    "The AI returned an empty response."));
        }

        try
        {
            using var document = JsonDocument.Parse(llmResponse);

            return Result<ResumeParsingResponse>.Success(
                new ResumeParsingResponse
                {
                    GeneratedJson = llmResponse,
                    StructuredContent = document.RootElement.Clone()
                });
        }
        catch (JsonException)
        {
            return Result<ResumeParsingResponse>.Failure(
                Error.Failure(
                    "ResumeParsing.InvalidJson",
                    "The AI response is not valid JSON."));
        }
    }
}