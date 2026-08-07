using TAO.AI.ResumeParsing.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.ResumeParsing.Validators;

internal sealed class ResumeResponseValidator
{
    public Result Validate(
        ResumeParsingResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return Result.Success();
    }
}