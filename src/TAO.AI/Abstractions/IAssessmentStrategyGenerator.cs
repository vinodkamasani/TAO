using TAO.AI.AssessmentStrategies.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IAssessmentStrategyGenerator
{
    Task<Result<AssessmentStrategyGenerationResult>> GenerateAsync(
        JobProfile jobProfile,
        HiringStrategy hiringStrategy,
        CancellationToken cancellationToken = default);
}