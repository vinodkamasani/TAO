using TAO.Domain.Enums;

namespace TAO.Application.Users.List;

public sealed record UserListItemDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Status,
    DateTime CreatedOn);