using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Organizations.Create;

public sealed record CreateOrganizationCommand(
    string Name,
    string Code)
    : IRequest<Result<Guid>>;