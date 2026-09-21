namespace TAO.Api.Endpoints.Auth.RegisterOrganization;

public sealed record RegisterOrganizationRequest(
    string OrganizationName,
    string OrganizationCode,
    string FirstName,
    string LastName,
    string Email,
    string Password);