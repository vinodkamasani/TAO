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
                x.CandidateName,
                x.Email
            })
            .ToListAsync(cancellationToken);

        foreach (var candidate in candidates)
        {
            var subject = "Your Application Has Been Recommended";

            var body =
                $"Hello {candidate.CandidateName},\n\n" +
                "Thank you for your interest in this opportunity. " +
                "Your application has been recommended for further consideration.\n\n" +
                "We will be in touch with the next steps.\n\n" +
                "Regards,\n" +
                "TAO";

            await _emailSender.SendAsync(
                candidate.Email,
                subject,
                body,
                cancellationToken);
        }

        return Result<SendRecommendedEmailsResponse>.Success(
            new SendRecommendedEmailsResponse(
                request.CampaignId,
                candidates.Count,
                candidates.Count));
    }
}