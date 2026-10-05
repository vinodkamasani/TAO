using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Candidates.Signup;

public sealed class CandidateSignupCommandHandler(
    IApplicationDbContext dbContext,
    IIdentityService identityService,
    ITransactionManager transactionManager)
    : IRequestHandler<CandidateSignupCommand, Result>
{
    public async Task<Result> Handle(
        CandidateSignupCommand request,
        CancellationToken cancellationToken)
    {
        var invitation = await dbContext
            .Set<CandidateInvitation>()
            .FirstOrDefaultAsync(
                x => x.Id == request.InvitationId,
                cancellationToken);

        if (invitation is null)
        {
            return Result.Failure(
                new Error(
                    "CandidateInvitation.NotFound",
                    "The candidate invitation could not be found."));
        }

        if (invitation.Status is
            CandidateInvitationStatus.Accepted or
            CandidateInvitationStatus.Revoked)
        {
            return Result.Failure(
                new Error(
                    "CandidateInvitation.InvalidStatus",
                    "This candidate invitation is no longer available."));
        }

        if (invitation.ExpiresOn is not null &&
            invitation.ExpiresOn <= DateTime.UtcNow)
        {
            return Result.Failure(
                new Error(
                    "CandidateInvitation.Expired",
                    "This candidate invitation has expired."));
        }

        var candidateApplication = await dbContext
            .Set<CandidateApplication>()
            .FirstOrDefaultAsync(
                x => x.Id == invitation.CandidateApplicationId,
                cancellationToken);

        if (candidateApplication is null)
        {
            return Result.Failure(
                new Error(
                    "CandidateApplication.NotFound",
                    "The candidate application could not be found."));
        }

        if (candidateApplication.IdentityUserId is not null)
        {
            return Result.Failure(
                new Error(
                    "Candidate.IdentityAlreadyRegistered",
                    "An account has already been created for this candidate."));
        }

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        if (!string.Equals(
                email,
                candidateApplication.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure(
                new Error(
                    "Candidate.EmailMismatch",
                    "The email address does not match the candidate invitation."));
        }

        var transactionResult =
     await transactionManager.ExecuteAsync(
         async transactionCancellationToken =>
         {
             var userId = Guid.CreateVersion7();

             var identityResult =
                 await identityService.CreateUserAsync(
                     userId,
                     email,
                     request.Password,
                     transactionCancellationToken);

             if (identityResult.IsFailure)
             {
                 return Result<bool>.Failure(
                     identityResult.Error);
             }

             var roleResult =
                 await identityService.AddToRoleAsync(
                     userId,
                     UserRole.Candidate.ToString(),
                     transactionCancellationToken);

             if (roleResult.IsFailure)
             {
                 return Result<bool>.Failure(
                     roleResult.Error);
             }

             candidateApplication.LinkIdentityUser(userId);

             invitation.Accept(DateTime.UtcNow);

             await dbContext.SaveChangesAsync(
                 transactionCancellationToken);

             return Result<bool>.Success(true);
         },
         cancellationToken);

        if (transactionResult.IsFailure)
        {
            return Result.Failure(
                transactionResult.Error);
        }

        return Result.Success();
    }
}