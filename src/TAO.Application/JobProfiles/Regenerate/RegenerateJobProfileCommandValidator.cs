using FluentValidation;

namespace TAO.Application.JobProfiles.Regenerate;

public sealed class RegenerateJobProfileCommandValidator
    : AbstractValidator<RegenerateJobProfileCommand>
{
    public RegenerateJobProfileCommandValidator()
    {
        RuleFor(x => x.JobProfileId)
            .NotEmpty();

        RuleFor(x => x.OriginalJobDescription)
            .NotEmpty()
            .MaximumLength(50000);
    }
}