using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Enums;

namespace TAO.Infrastructure.Identity;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User =>
        httpContextAccessor.HttpContext?.User;

    public Guid? UserId =>
        GetGuidClaim(ClaimTypes.NameIdentifier);

    public Guid? OrganizationId =>
        GetGuidClaim("OrganizationId");

    public UserRole? Role =>
        GetEnumClaim<UserRole>(ClaimTypes.Role);

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    private Guid? GetGuidClaim(string claimType)
    {
        var value = User?.FindFirstValue(claimType);

        return Guid.TryParse(value, out var id)
            ? id
            : null;
    }

    private TEnum? GetEnumClaim<TEnum>(string claimType)
        where TEnum : struct, Enum
    {
        var value = User?.FindFirstValue(claimType);

        return Enum.TryParse<TEnum>(
            value,
            ignoreCase: true,
            out var result)
            ? result
            : null;
    }
}