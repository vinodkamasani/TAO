using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Users.List;

public sealed class GetUsersHandler(
    IApplicationDbContext dbContext,
    ICurrentUser currentUser)
    : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserListItemDto>>>
{
    public async Task<Result<IReadOnlyList<UserListItemDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null)
        {
            return Result<IReadOnlyList<UserListItemDto>>.Failure(
                new Error(
                    "User.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId = await dbContext
            .Set<User>()
            .Where(x => x.Id == currentUser.UserId.Value)
            .Select(x => (Guid?)x.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (organizationId is null)
        {
            return Result<IReadOnlyList<UserListItemDto>>.Failure(
                new Error(
                    "User.CurrentUserNotFound",
                    "The current user could not be found."));
        }

        var users = await dbContext
            .Set<User>()
            .AsNoTracking()
            .Where(x => x.OrganizationId == organizationId.Value)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Select(x => new UserListItemDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Role,
                x.Status,
                x.CreatedOn))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<UserListItemDto>>.Success(users);
    }
}