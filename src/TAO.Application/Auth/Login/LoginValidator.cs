using FluentValidation;

namespace TAO.Application.Auth.Login;

public sealed class LoginValidator
    : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}