namespace TAO.Application.Auth.Contracts;

public sealed record UserResponse(
    Guid Id,
    Guid OrganizationId,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Status);