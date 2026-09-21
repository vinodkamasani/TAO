using Microsoft.AspNetCore.Identity;
using TAO.Application.Common.Interfaces;

namespace TAO.Infrastructure.Identity;

public sealed class AuthenticationService(
    SignInManager<ApplicationUser> signInManager)
    : IAuthenticationService
{
    public async Task<bool> SignInAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var result = await signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: false,
            lockoutOnFailure: true);

        return result.Succeeded;
    }

    public async Task SignOutAsync(
        CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();
    }
}