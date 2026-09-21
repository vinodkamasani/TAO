namespace TAO.Application.Auth.Login;

public sealed record LoginResponse(
    Guid UserId,
    Guid OrganizationId,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Status);