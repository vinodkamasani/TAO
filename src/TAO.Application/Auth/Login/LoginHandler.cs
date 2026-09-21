using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Login;

public sealed class LoginHandler(
    IAuthenticationService authenticationService,
    IApplicationDbContext dbContext)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
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

        if (user is null)
        {
            return Result<LoginResponse>.Failure(
                new Error(
                    "Auth.InvalidCredentials",
                    "Invalid email or password."));
        }

        if (user.Status != UserStatus.Active)
        {
            return Result<LoginResponse>.Failure(
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
            return Result<LoginResponse>.Failure(
                new Error(
                    "Auth.InvalidCredentials",
                    "Invalid email or password."));
        }

        var response = new LoginResponse(
            user.Id,
            user.OrganizationId,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role.ToString(),
            user.Status.ToString());

        return Result<LoginResponse>.Success(response);
    }
}