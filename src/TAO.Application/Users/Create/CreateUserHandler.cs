using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Application.Users.Common;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Users.Create;

public sealed class CreateUserHandler(
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IIdentityService identityService,
    ITransactionManager transactionManager)
    : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null)
        {
            return Result<UserDto>.Failure(
                new Error(
                    "User.Unauthorized",
                    "The current user is not authenticated."));
        }

        var currentUserEntity = await dbContext
            .Set<User>()
            .FirstOrDefaultAsync(
                x => x.Id == currentUser.UserId.Value,
                cancellationToken);

        if (currentUserEntity is null)
        {
            return Result<UserDto>.Failure(
                new Error(
                    "User.CurrentUserNotFound",
                    "The current user could not be found."));
        }

        if (currentUserEntity.Role != UserRole.Administrator)
        {
            return Result<UserDto>.Failure(
                new Error(
                    "User.Forbidden",
                    "Only organization administrators can create users."));
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await dbContext
            .Set<User>()
            .AnyAsync(
                x =>
                    x.OrganizationId ==
                    currentUserEntity.OrganizationId &&
                    x.Email == email,
                cancellationToken);

        if (exists)
        {
            return Result<UserDto>.Failure(
                new Error(
                    "User.DuplicateEmail",
                    "A user with the same email already exists."));
        }

        return await transactionManager.ExecuteAsync(
            async transactionCancellationToken =>
            {
                var userId = Guid.CreateVersion7();

                var identityResult =
                    await identityService.CreateUserAsync(
                        userId,
                        email,
                        request.Password,
                        transactionCancellationToken);

                if (!identityResult.IsSuccess)
                {
                    return Result<UserDto>.Failure(
                        identityResult.Error);
                }

                var user = new User(
                    currentUserEntity.OrganizationId,
                    request.FirstName,
                    request.LastName,
                    email,
                    request.Role)
                {
                    Id = userId
                };

                await dbContext
                    .Set<User>()
                    .AddAsync(
                        user,
                        transactionCancellationToken);

                await dbContext.SaveChangesAsync(
                    transactionCancellationToken);

                var response = new UserDto(
                    user.Id,
                    user.OrganizationId,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role,
                    user.Status.ToString(),
                    user.CreatedOn);

                return Result<UserDto>.Success(response);
            },
            cancellationToken);
    }
}