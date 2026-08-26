using FluentValidation;

namespace TAO.Application.AssessmentQuestions.CandidateResponse;

public sealed class RecordCandidateResponseCommandValidator
: AbstractValidator<RecordCandidateResponseCommand>
{
    public RecordCandidateResponseCommandValidator()
    {
        RuleFor(x => x.AssessmentQuestionId)
            .NotEmpty();

        RuleFor(x => x.Response)
            .NotEmpty()
            .MaximumLength(10000);
    }
}
