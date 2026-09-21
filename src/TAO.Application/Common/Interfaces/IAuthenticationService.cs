namespace TAO.Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<bool> SignInAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    Task SignOutAsync(
        CancellationToken cancellationToken);
}