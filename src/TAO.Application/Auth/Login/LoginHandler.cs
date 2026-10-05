using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Auth.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Login;

public sealed class LoginHandler(
    IAuthenticationService authenticationService,
    IApplicationDbContext dbContext)
    : IRequestHandler<LoginCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var user = await dbContext
            .Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

        if (user is not null)
        {
            if (user.Status != UserStatus.Active)
            {
                return Result<UserResponse>.Failure(
                    new Error(
                        "Auth.UserInactive",
                        "The user account is inactive."));
            }

            var authenticated =
                await authenticationService.SignInAsync(
                    email,
                    request.Password,
                    cancellationToken);

            if (!authenticated)
            {
                return Result<UserResponse>.Failure(
                    new Error(
                        "Auth.InvalidCredentials",
                        "Invalid email or password."));
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

        var candidateApplication = await dbContext
            .Set<CandidateApplication>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Email == email &&
                    x.IdentityUserId != null,
                cancellationToken);

        if (candidateApplication is null)
        {
            return Result<UserResponse>.Failure(
                new Error(
                    "Auth.InvalidCredentials",
                    "Invalid email or password."));
        }

        var candidateAuthenticated =
            await authenticationService.SignInAsync(
                email,
                request.Password,
                cancellationToken);

        if (!candidateAuthenticated)
        {
            return Result<UserResponse>.Failure(
                new Error(
                    "Auth.InvalidCredentials",
                    "Invalid email or password."));
        }

        return Result<UserResponse>.Success(
            new UserResponse(
                candidateApplication.IdentityUserId!.Value,
                null,
                candidateApplication.CandidateName,
                string.Empty,
                candidateApplication.Email,
                UserRole.Candidate.ToString(),
                "Active"));
    }
}