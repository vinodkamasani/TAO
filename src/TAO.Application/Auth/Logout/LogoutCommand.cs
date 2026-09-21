using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Logout;

public sealed record LogoutCommand
    : IRequest<Result>;