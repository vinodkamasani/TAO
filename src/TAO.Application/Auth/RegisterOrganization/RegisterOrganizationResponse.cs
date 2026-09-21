using TAO.Domain.Enums;

namespace TAO.Application.Auth.RegisterOrganization;

public sealed record RegisterOrganizationResponse(
    Guid OrganizationId,
    Guid UserId,
    string OrganizationName,
    string OrganizationCode,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    UserStatus Status);