using MediatR;
using TAO.Application.Common.Interfaces;
using TAO.SharedKernel.Results;

namespace TAO.Application.Auth.Logout;

public sealed class LogoutHandler(
    IAuthenticationService authenticationService)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        await authenticationService.SignOutAsync(
            cancellationToken);

        return Result.Success();
    }
}