using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Users.List;

public sealed record GetUsersQuery
    : IRequest<Result<IReadOnlyList<UserListItemDto>>>;