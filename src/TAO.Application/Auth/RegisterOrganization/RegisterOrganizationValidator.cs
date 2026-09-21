using FluentValidation;

namespace TAO.Application.Auth.RegisterOrganization;

public sealed class RegisterOrganizationValidator
    : AbstractValidator<RegisterOrganizationCommand>
{
    public RegisterOrganizationValidator()
    {
        RuleFor(x => x.OrganizationName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.OrganizationCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9-]+$");

        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(8);
    }
}