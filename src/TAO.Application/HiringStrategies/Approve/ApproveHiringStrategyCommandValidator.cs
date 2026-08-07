using FluentValidation;

namespace TAO.Application.HiringStrategies.Approve;

public sealed class ApproveHiringStrategyCommandValidator
    : AbstractValidator<ApproveHiringStrategyCommand>
{
    public ApproveHiringStrategyCommandValidator()
    {
        RuleFor(x => x.HiringStrategyId)
            .NotEmpty();
    }
}