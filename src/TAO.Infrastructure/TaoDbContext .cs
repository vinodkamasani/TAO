using Microsoft.EntityFrameworkCore;

namespace TAO.Infrastructure;

public sealed class TaoDbContext : DbContext
{
    public TaoDbContext(DbContextOptions<TaoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TaoDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}