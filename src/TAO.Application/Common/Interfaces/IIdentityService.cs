using TAO.SharedKernel.Results;

namespace TAO.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> CreateUserAsync(
        Guid userId,
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<Result> AddToRoleAsync(
       Guid userId,
       string role,
       CancellationToken cancellationToken);
}