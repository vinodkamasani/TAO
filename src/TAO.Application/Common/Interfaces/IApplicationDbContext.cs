using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TAO.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;


    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    EntityEntry Entry(object entity);
}