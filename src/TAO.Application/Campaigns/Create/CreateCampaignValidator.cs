using FluentValidation;

namespace TAO.Application.Campaigns.Create;

public sealed class CreateCampaignValidator
    : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ReferenceNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.RecruiterId)
            .NotEmpty();

        RuleFor(x => x.HiringManagerId)
            .NotEmpty();

        RuleFor(x => x.NumberOfOpenings)
            .GreaterThan(0);
    }
}