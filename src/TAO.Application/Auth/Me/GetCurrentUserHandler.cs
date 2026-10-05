using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Auth.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Me;

public sealed class GetCurrentUserHandler(
    ICurrentUser currentUser,
    IApplicationDbContext dbContext)
    : IRequestHandler<
        GetCurrentUserQuery,
        Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null)
        {
            return Result<UserResponse>.Failure(
                Error.Unauthorized(
                    "Auth.Unauthorized",
                    "The current user is not authenticated."));
        }

        if (currentUser.Role == UserRole.Candidate)
        {
            return await GetCandidateAsync(
                currentUser.UserId.Value,
                cancellationToken);
        }

        return await GetOrganizationUserAsync(
            currentUser.UserId.Value,
            cancellationToken);
    }

    private async Task<Result<UserResponse>> GetOrganizationUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await dbContext
            .Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken);

        if (user is null)
        {
            return Result<UserResponse>.Failure(
                Error.NotFound(
                    "Auth.UserNotFound",
                    "The authenticated user could not be found."));
        }

        return Result<UserResponse>.Success(
            new UserResponse(
                user.Id,
                user.OrganizationId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role.ToString(),
                user.Status.ToString()));
    }

    private async Task<Result<UserResponse>> GetCandidateAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var candidateApplication = await dbContext
            .Set<CandidateApplication>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.IdentityUserId == userId,
                cancellationToken);

        if (candidateApplication is null)
        {
            return Result<UserResponse>.Failure(
                Error.NotFound(
                    "Auth.CandidateNotFound",
                    "The authenticated candidate could not be found."));
        }

        return Result<UserResponse>.Success(
            new UserResponse(
                userId,
                candidateApplication.OrganizationId,
                candidateApplication.CandidateName,
                string.Empty,
                candidateApplication.Email,
                UserRole.Candidate.ToString(),
                "Active"));
    }
}