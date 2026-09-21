using TAO.Domain.Enums;

namespace TAO.Application.Auth.Me;

public sealed record GetCurrentUserResponse(
    Guid Id,
    Guid OrganizationId,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    UserStatus Status);