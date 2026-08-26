using FluentValidation;

namespace TAO.Application.AssessmentStrategies.Approve;

public sealed class ApproveAssessmentStrategyCommandValidator
    : AbstractValidator<ApproveAssessmentStrategyCommand>
{
    public ApproveAssessmentStrategyCommandValidator()
    {
        RuleFor(x => x.AssessmentStrategyId)
            .NotEmpty();

        RuleFor(x => x.ApprovedByUserId)
            .NotEmpty();
    }
}