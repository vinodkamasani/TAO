using FluentValidation;

namespace TAO.Application.CandidateApplications.SendRecommendedEmails;

public sealed class SendRecommendedEmailsCommandValidator
    : AbstractValidator<SendRecommendedEmailsCommand>
{
    public SendRecommendedEmailsCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty()
            .WithMessage("CampaignId is required.");
    }
}