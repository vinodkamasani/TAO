using FluentValidation;

namespace TAO.Application.ResumeScreenings.ScreenCampaign;

public sealed class ScreenCampaignResumesCommandValidator
    : AbstractValidator<ScreenCampaignResumesCommand>
{
    public ScreenCampaignResumesCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty()
            .WithMessage("Campaign Id is required.");
    }
}