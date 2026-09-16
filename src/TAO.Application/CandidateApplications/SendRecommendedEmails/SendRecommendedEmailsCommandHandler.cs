using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.CandidateApplications.SendRecommendedEmails;

internal sealed class SendRecommendedEmailsCommandHandler
    : IRequestHandler<
        SendRecommendedEmailsCommand,
        Result<SendRecommendedEmailsResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSender _emailSender;

    public SendRecommendedEmailsCommandHandler(
        IApplicationDbContext context,
        IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public async Task<Result<SendRecommendedEmailsResponse>> Handle(
        SendRecommendedEmailsCommand request,
        CancellationToken cancellationToken)
    {
        var campaignExists = await _context
            .Set<Domain.Entities.Campaign>()
            .AnyAsync(
                x => x.Id == request.CampaignId,
                cancellationToken);

        if (!campaignExists)
        {
            return Result<SendRecommendedEmailsResponse>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign '{request.CampaignId}' was not found."));
        }

        var candidates = await _context
            .Set<CandidateApplication>()
            .AsNoTracking()
            .Where(x =>
                x.CampaignId == request.CampaignId &&
                x.IsRecommended)
            .Select(x => new
            {
                x.Id,
                x.OrganizationId,
                x.CandidateName,
                x.Email
            })
            .ToListAsync(cancellationToken);

        const string subject =
            "Your Application Has Been Recommended";

        var sentCount = 0;

        foreach (var candidate in candidates)
        {
            var body =
                $"Hello {candidate.CandidateName},\n\n" +
                "Thank you for your interest in this opportunity. " +
                "Your application has been recommended for further consideration.\n\n" +
                "We will be in touch with the next steps.\n\n" +
                "Regards,\n" +
                "TAO";

            var emailDelivery = EmailDelivery.Create(
                candidate.OrganizationId,
                request.CampaignId,
                candidate.Id,
                candidate.Email,
                subject,
                body);

            _context.Set<EmailDelivery>().Add(emailDelivery);

            await _context.SaveChangesAsync(
                cancellationToken);

            try
            {
                await _emailSender.SendAsync(
                    candidate.Email,
                    subject,
                    body,
                    cancellationToken);

                emailDelivery.MarkAsSent(
                    DateTime.UtcNow);

                sentCount++;
            }
            catch (Exception ex)
            {
                emailDelivery.MarkAsFailed(
                    ex.Message,
                    DateTime.UtcNow);
            }

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        return Result<SendRecommendedEmailsResponse>.Success(
            new SendRecommendedEmailsResponse(
                request.CampaignId,
                candidates.Count,
                sentCount));
    }
}