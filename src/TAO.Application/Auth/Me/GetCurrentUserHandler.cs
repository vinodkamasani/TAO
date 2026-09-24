using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Auth.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
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
                new Error(
                    "Auth.Unauthorized",
                    "The current user is not authenticated."));
        }

        var user = await dbContext
            .Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == currentUser.UserId.Value,
                cancellationToken);

        if (user is null)
        {
            return Result<UserResponse>.Failure(
                new Error(
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
}