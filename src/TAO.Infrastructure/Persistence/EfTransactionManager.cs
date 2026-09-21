using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.SharedKernel.Results;

namespace TAO.Infrastructure.Persistence;

public sealed class EfTransactionManager(
    TaoDbContext dbContext) : ITransactionManager
{
    public async Task<Result<T>> ExecuteAsync<T>(
        Func<CancellationToken, Task<Result<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var result = await operation(cancellationToken);

            if (!result.IsSuccess)
            {
                await transaction.RollbackAsync(cancellationToken);
                return result;
            }

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}