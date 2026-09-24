using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using TAO.Domain.Entities;

namespace TAO.Infrastructure.Identity;

public sealed class TaoUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    IOptions<IdentityOptions> optionsAccessor,
    TaoDbContext dbContext)
    : UserClaimsPrincipalFactory<ApplicationUser>(
        userManager,
        optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(
        ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var taoUser = await dbContext
            .Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == user.Id);

        if (taoUser is null)
        {
            return identity;
        }

        identity.AddClaim(
            new Claim(
                "OrganizationId",
                taoUser.OrganizationId.ToString()));

        identity.AddClaim(
            new Claim(
                ClaimTypes.Role,
                taoUser.Role.ToString()));

        identity.AddClaim(
            new Claim(
                ClaimTypes.Name,
                $"{taoUser.FirstName} {taoUser.LastName}"));

        return identity;
    }
}