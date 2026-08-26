using FluentValidation;

namespace TAO.Application.AssessmentQuestions.CodeResponse;

public sealed class RecordCodeResponseCommandValidator
    : AbstractValidator<RecordCodeResponseCommand>
{
    public RecordCodeResponseCommandValidator()
    {
        RuleFor(x => x.AssessmentQuestionId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(500_000);
    }
}