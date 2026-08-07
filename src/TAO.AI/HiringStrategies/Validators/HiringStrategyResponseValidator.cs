using System.Text.Json;
using TAO.AI.Common;
using TAO.AI.HiringStrategies.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.HiringStrategies.Validators;

internal sealed class HiringStrategyResponseValidator
{
    public Result Validate(HiringStrategyAiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (string.IsNullOrWhiteSpace(response.GeneratedMarkdown))
        {
            return Result.Failure(AiErrors.GeneratedMarkdownMissing);
        }

        if (response.StructuredContent.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure(AiErrors.InvalidStructuredProfile);
        }

        return Result.Success();
    }
}