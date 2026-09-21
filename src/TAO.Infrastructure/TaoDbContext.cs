using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Infrastructure.Identity;

namespace TAO.Infrastructure;

public sealed class TaoDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>,
      IApplicationDbContext
{
    public TaoDbContext(
        DbContextOptions<TaoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        // ASP.NET Identity tables
        modelBuilder.Entity<ApplicationUser>()
            .ToTable("IdentityUsers");

        modelBuilder.Entity<IdentityRole<Guid>>()
            .ToTable("IdentityRoles");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("IdentityUserClaims");

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("IdentityRoleClaims");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("IdentityUserLogins");

        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("IdentityUserRoles");

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("IdentityUserTokens");

        // TAO entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TaoDbContext).Assembly);
    }
}