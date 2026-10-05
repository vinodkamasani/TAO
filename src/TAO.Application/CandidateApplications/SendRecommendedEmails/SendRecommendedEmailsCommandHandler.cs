using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.CandidateApplications.SendRecommendedEmails;

internal sealed class SendRecommendedEmailsCommandHandler
    : IRequestHandler<
        SendRecommendedEmailsCommand,
        Result<SendRecommendedEmailsResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly ICurrentUser _currentUser;
    private readonly IConfiguration _configuration;

    public SendRecommendedEmailsCommandHandler(
        IApplicationDbContext context,
        IEmailSender emailSender,
        ICurrentUser currentUser,
        IConfiguration configuration)
    {
        _context = context;
        _emailSender = emailSender;
        _currentUser = currentUser;
        _configuration = configuration;
    }

    public async Task<Result<SendRecommendedEmailsResponse>> Handle(
        SendRecommendedEmailsCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<SendRecommendedEmailsResponse>.Failure(
                Error.Unauthorized(
                    "SendRecommendedEmails.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<SendRecommendedEmailsResponse>.Failure(
                Error.Unauthorized(
                    "SendRecommendedEmails.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        // ---------------------------------------------------------
        // 3. Validate campaign belongs to organization
        // ---------------------------------------------------------

        var campaignExists = await _context
            .Set<Campaign>()
            .AnyAsync(
                x =>
                    x.Id == request.CampaignId &&
                    x.OrganizationId == organizationId.Value,
                cancellationToken);

        if (!campaignExists)
        {
            return Result<SendRecommendedEmailsResponse>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign '{request.CampaignId}' was not found."));
        }

        // ---------------------------------------------------------
        // 4. Get recommended candidates
        // ---------------------------------------------------------

        var candidates = await _context
            .Set<CandidateApplication>()
            .AsNoTracking()
            .Where(x =>
                x.OrganizationId == organizationId.Value &&
                x.CampaignId == request.CampaignId &&
                x.IsRecommended)
            .Select(x => new
            {
                x.Id,
                x.OrganizationId,
                x.CampaignId,
                x.CandidateName,
                x.Email
            })
            .ToListAsync(cancellationToken);

        var candidatePortalBaseUrl =
            _configuration["CandidatePortal:BaseUrl"];

        if (string.IsNullOrWhiteSpace(candidatePortalBaseUrl))
        {
            return Result<SendRecommendedEmailsResponse>.Failure(
                new Error(
                    "CandidateInvitation.PortalUrlNotConfigured",
                    "The candidate portal URL has not been configured."));
        }

        candidatePortalBaseUrl =
            candidatePortalBaseUrl.TrimEnd('/');

        const string subject =
            "You Have Been Invited to Complete an Assessment";

        var sentCount = 0;

        foreach (var candidate in candidates)
        {
            // -----------------------------------------------------
            // 5. Check whether an invitation already exists
            // -----------------------------------------------------

            var invitation = await _context
                .Set<CandidateInvitation>()
                .FirstOrDefaultAsync(
                    x =>
                        x.CandidateApplicationId == candidate.Id,
                    cancellationToken);

            if (invitation is null)
            {
                invitation = CandidateInvitation.Create(
                    candidate.OrganizationId,
                    candidate.CampaignId,
                    candidate.Id,
                    expiresOn: DateTime.UtcNow.AddDays(7));

                _context
                    .Set<CandidateInvitation>()
                    .Add(invitation);

                await _context.SaveChangesAsync(
                    cancellationToken);
            }

            // -----------------------------------------------------
            // 6. Don't create duplicate invitations
            // -----------------------------------------------------

            if (invitation.Status is
                CandidateInvitationStatus.Accepted or
                CandidateInvitationStatus.Revoked)
            {
                continue;
            }

            if (invitation.ExpiresOn is not null &&
                invitation.ExpiresOn <= DateTime.UtcNow)
            {
                invitation.Expire();

                await _context.SaveChangesAsync(
                    cancellationToken);

                continue;
            }

            // -----------------------------------------------------
            // 7. Build candidate invitation URL
            // -----------------------------------------------------

            var invitationUrl =
                $"{candidatePortalBaseUrl}/candidate/invitations/{invitation.Id}";



            var body =
                  $"Hello {candidate.CandidateName},\n\n" +
                  "Your application has been recommended for further consideration " +
                  "and you have been invited to complete an assessment.\n\n" +
                  "Please use the following link to continue:\n\n" +
                  $"{invitationUrl}\n\n" +
                  "Regards,\n" +
                  "TAO";

            // -----------------------------------------------------
            // 8. Create email delivery record
            // -----------------------------------------------------

            var emailDelivery = EmailDelivery.Create(
                candidate.OrganizationId,
                candidate.CampaignId,
                candidate.Id,
                candidate.Email,
                subject,
                body);

            _context
                .Set<EmailDelivery>()
                .Add(emailDelivery);

            await _context.SaveChangesAsync(
                cancellationToken);

            // -----------------------------------------------------
            // 9. Send email
            // -----------------------------------------------------

            try
            {
                await _emailSender.SendAsync(
                    candidate.Email,
                    subject,
                    body,
                    cancellationToken);

                emailDelivery.MarkAsSent(
                    DateTime.UtcNow);

                invitation.MarkAsSent(
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