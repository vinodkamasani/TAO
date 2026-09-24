using MediatR;
using TAO.Application.Auth.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Me;

public sealed record GetCurrentUserQuery
    : IRequest<Result<UserResponse>>;