using TAO.AI.JobProfiles.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IJobProfileGenerator
{
    Task<Result<JobProfileGenerationResult>> GenerateAsync(
        string jobDescription,
        CancellationToken cancellationToken = default);
}