using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TAO.Application.Common.Interfaces;

namespace TAO.Infrastructure.Identity;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor
            .HttpContext?
            .User?
            .Identity?
            .IsAuthenticated == true;
}