using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.RegisterOrganization;

public sealed record RegisterOrganizationCommand(
    string OrganizationName,
    string OrganizationCode,
    string FirstName,
    string LastName,
    string Email,
    string Password)
    : IRequest<Result<RegisterOrganizationResponse>>;