using TAO.AI.HiringStrategies.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IHiringStrategyGenerator
{
    Task<Result<HiringStrategyGenerationResult>> GenerateAsync(
        JobProfile jobProfile,
        CancellationToken cancellationToken = default);
}