using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Me;

public sealed record GetCurrentUserQuery
    : IRequest<Result<GetCurrentUserResponse>>;