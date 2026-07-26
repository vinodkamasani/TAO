using FluentValidation;

namespace TAO.Application.Organizations.Create;

public sealed class CreateOrganizationValidator
    : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);
    }
}