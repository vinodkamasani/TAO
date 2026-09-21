using TAO.Domain.Enums;

namespace TAO.Api.Endpoints.Users.Create;

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role);