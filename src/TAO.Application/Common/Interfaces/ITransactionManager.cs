using TAO.SharedKernel.Results;

namespace TAO.Application.Common.Interfaces;

public interface ITransactionManager
{
    Task<Result<T>> ExecuteAsync<T>(
        Func<CancellationToken, Task<Result<T>>> operation,
        CancellationToken cancellationToken = default);
}