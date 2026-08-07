using FluentValidation;

namespace TAO.Application.ResumeScreenings.Create;

public sealed class CreateResumeScreeningCommandValidator
    : AbstractValidator<CreateResumeScreeningCommand>
{
    public CreateResumeScreeningCommandValidator()
    {
        RuleFor(x => x.CandidateApplicationId)
            .NotEmpty()
            .WithMessage("Candidate Application Id is required.");
    }
}