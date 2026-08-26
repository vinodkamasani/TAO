using FluentValidation;

namespace TAO.Application.AssessmentStrategies.Create;

public sealed class CreateAssessmentStrategyValidator
    : AbstractValidator<CreateAssessmentStrategyCommand>
{
    public CreateAssessmentStrategyValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty()
            .WithMessage("CampaignId is required.");
    }
}