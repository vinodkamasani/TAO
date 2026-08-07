using System.Text.Json;
using TAO.AI.ResumeScreening.Models;
using TAO.SharedKernel.Results;

namespace TAO.AI.ResumeScreening.Parsers;

internal sealed class ResumeScreeningResponseParser
{
    public Result<ResumeScreeningResponse> Parse(
        string response)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(response);

        try
        {
            using var document = JsonDocument.Parse(response);

            var root = document.RootElement;

            var screeningResponse =
                new ResumeScreeningResponse
                {
                    OverallMatchPercentage =
                        root.GetProperty("overallMatchPercentage")
                            .GetByte(),

                    IsRecommended =
                        root.GetProperty("isRecommended")
                            .GetBoolean(),

                    GeneratedMarkdown = string.Empty,

                    StructuredContent =
                        root.GetProperty("structuredContent")
                            .Clone()
                };

            return Result<ResumeScreeningResponse>.Success(
                screeningResponse);
        }
        catch (Exception ex)
        {
            return Result<ResumeScreeningResponse>.Failure(
                Error.Failure(
                    "ResumeScreening.InvalidResponse",
                    $"Failed to parse AI response. {ex.Message}"));
        }
    }
}
