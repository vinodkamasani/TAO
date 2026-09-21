using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Me;

public sealed class GetCurrentUserHandler(
    ICurrentUser currentUser,
    IApplicationDbContext dbContext)
    : IRequestHandler<
        GetCurrentUserQuery,
        Result<GetCurrentUserResponse>>
{
    public async Task<Result<GetCurrentUserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null)
        {
            return Result<GetCurrentUserResponse>.Failure(
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
            return Result<GetCurrentUserResponse>.Failure(
                new Error(
                    "Auth.UserNotFound",
                    "The authenticated user could not be found."));
        }

        return Result<GetCurrentUserResponse>.Success(
            new GetCurrentUserResponse(
                user.Id,
                user.OrganizationId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.Status));
    }
}