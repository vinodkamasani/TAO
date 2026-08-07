using FluentValidation;

namespace TAO.Application.ResumeImports.Create;

public sealed class CreateResumeImportCommandValidator
    : AbstractValidator<CreateResumeImportCommand>
{
    public CreateResumeImportCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty();

        RuleFor(x => x.Resumes)
            .NotEmpty();
    }
}