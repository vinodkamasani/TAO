using FluentValidation;

namespace TAO.Application.JobProfiles.Create;

public sealed class CreateJobProfileCommandValidator
 : AbstractValidator<CreateJobProfileCommand>
{
    public CreateJobProfileCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty();

        RuleFor(x => x.OriginalJobDescription)
            .NotEmpty()
            .MaximumLength(50000);
    }
}
