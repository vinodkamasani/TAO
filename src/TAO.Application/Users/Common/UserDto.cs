using TAO.Domain.Enums;

namespace TAO.Application.Users.Common;

public sealed record UserDto(
    Guid Id,
    Guid OrganizationId,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    string Status,
    DateTime CreatedOn);