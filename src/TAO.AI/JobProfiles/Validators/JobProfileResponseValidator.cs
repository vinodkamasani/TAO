using System.Text.Json;
using TAO.AI.Common;
using TAO.AI.JobProfiles.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.JobProfiles.Validators;

internal sealed class JobProfileResponseValidator 
{
    public Result Validate(JobProfileAiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (string.IsNullOrWhiteSpace(response.GeneratedMarkdown))
        {
            return Result.Failure(AiErrors.GeneratedMarkdownMissing);
        }

        if (response.StructuredProfile.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure(AiErrors.InvalidStructuredProfile);
        }

        if (!response.StructuredProfile.TryGetProperty("roleTitle", out _))
        {
            return Result.Failure(AiErrors.RoleTitleMissing);
        }

        if (!response.StructuredProfile.TryGetProperty("requiredSkills", out _))
        {
            return Result.Failure(AiErrors.RequiredSkillsMissing);
        }

        return Result.Success();
    }
}