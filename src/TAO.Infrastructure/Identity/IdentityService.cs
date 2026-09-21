using Microsoft.AspNetCore.Identity;
using TAO.Application.Common.Interfaces;
using TAO.SharedKernel.Results;

namespace TAO.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager)
    : IIdentityService
{
    public async Task<Result<Guid>> CreateUserAsync(
        Guid userId,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = email,
            Email = email
        };

        var result = await userManager.CreateAsync(
            user,
            password);

        if (result.Succeeded)
        {
            return Result<Guid>.Success(userId);
        }

        var errors = string.Join(
            "; ",
            result.Errors.Select(x => x.Description));

        return Result<Guid>.Failure(
            new Error(
                "Identity.UserCreationFailed",
                errors));
    }

}