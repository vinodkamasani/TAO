using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<Result<LoginResponse>>;