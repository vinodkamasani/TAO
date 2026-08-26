using FluentValidation;

namespace TAO.Application.AssessmentSessions.Create;

public sealed class CreateAssessmentSessionCommandValidator
    : AbstractValidator<CreateAssessmentSessionCommand>
{
    public CreateAssessmentSessionCommandValidator()
    {
        RuleFor(x => x.CandidateApplicationId)
            .NotEmpty()
            .WithMessage("Candidate application is required.");

        RuleFor(x => x.AssessmentStrategyId)
            .NotEmpty()
            .WithMessage("Assessment strategy is required.");
    }
}