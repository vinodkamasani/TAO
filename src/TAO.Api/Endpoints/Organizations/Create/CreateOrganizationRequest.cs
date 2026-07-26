namespace TAO.Api.Endpoints.Organizations.Create;

public sealed record CreateOrganizationRequest(
    string Name,
    string Code);