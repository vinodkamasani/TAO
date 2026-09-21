using TAO.Domain.Enums;

namespace TAO.Application.Users.List;

public sealed record UserListItemDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    UserStatus Status,
    DateTime CreatedOn);