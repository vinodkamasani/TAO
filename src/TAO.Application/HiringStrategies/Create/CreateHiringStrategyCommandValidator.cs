using FluentValidation;

namespace TAO.Application.HiringStrategies.Create;

public sealed class CreateHiringStrategyCommandValidator
    : AbstractValidator<CreateHiringStrategyCommand>
{
    public CreateHiringStrategyCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty();
    }
}