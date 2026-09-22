using FluentValidation;

namespace TAO.Application.AssessmentStrategies.Update;

public sealed class UpdateAssessmentStrategyValidator
    : AbstractValidator<UpdateAssessmentStrategyCommand>
{
    public UpdateAssessmentStrategyValidator()
    {
        RuleFor(x => x.AssessmentStrategyId)
            .NotEmpty();

        RuleFor(x => x.AssessmentName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Rounds)
            .NotEmpty();

        RuleForEach(x => x.Rounds)
            .SetValidator(new UpdateAssessmentRoundValidator());
    }

    private sealed class UpdateAssessmentRoundValidator
        : AbstractValidator<UpdateAssessmentRoundCommand>
    {
        public UpdateAssessmentRoundValidator()
        {
            RuleFor(x => x.Order)
                .GreaterThan(0);

            RuleFor(x => x.Type)
                .NotEmpty();

            RuleFor(x => x.Difficulty)
                .NotEmpty();

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0);

            RuleFor(x => x.QuestionCount)
                .GreaterThan(0);

            RuleFor(x => x.Competencies)
                .NotNull();

            RuleForEach(x => x.Competencies)
                .SetValidator(
                    new UpdateAssessmentCompetencyValidator());
        }
    }

    private sealed class UpdateAssessmentCompetencyValidator
        : AbstractValidator<UpdateAssessmentCompetencyCommand>
    {
        public UpdateAssessmentCompetencyValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Priority)
                .NotEmpty();

            RuleFor(x => x.MinimumPassPercentage)
                .InclusiveBetween((byte)0, (byte)100);
        }
    }
}