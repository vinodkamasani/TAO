using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TAO.Domain.Entities;
using TAO.Domain.Enums;

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

        if (taoUser is not null)
        {
            AddOrganizationUserClaims(
                identity,
                taoUser);

            return identity;
        }

        var candidateApplication = await dbContext
            .Set<CandidateApplication>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.IdentityUserId == user.Id);

        if (candidateApplication is not null)
        {
            AddCandidateClaims(
                identity,
                candidateApplication);
        }

        return identity;
    }

    private static void AddOrganizationUserClaims(
        ClaimsIdentity identity,
        User user)
    {
        identity.AddClaim(
            new Claim(
                "OrganizationId",
                user.OrganizationId.ToString()));

        identity.AddClaim(
            new Claim(
                ClaimTypes.Role,
                user.Role.ToString()));

        identity.AddClaim(
            new Claim(
                ClaimTypes.Name,
                $"{user.FirstName} {user.LastName}"));
    }

    private static void AddCandidateClaims(
        ClaimsIdentity identity,
        CandidateApplication candidateApplication)
    {
        identity.AddClaim(
            new Claim(
                ClaimTypes.Role,
                UserRole.Candidate.ToString()));

        identity.AddClaim(
            new Claim(
                ClaimTypes.Name,
                candidateApplication.CandidateName));
    }
}