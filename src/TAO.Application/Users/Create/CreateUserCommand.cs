using MediatR;
using TAO.Application.Users.Common;
using TAO.Domain.Enums;
using TAO.SharedKernel.Results;

namespace TAO.Application.Users.Create;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role)
    : IRequest<Result<UserDto>>;