namespace TAO.Api.Endpoints.Auth.Login;

public sealed record LoginRequest(
    string Email,
    string Password);