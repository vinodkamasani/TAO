using TAO.AI.ResumeScreening.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IResumeScreeningGenerator
{
    Task<Result<ResumeScreeningResult>> GenerateAsync(
        ResumeScreeningRequest request,
        CancellationToken cancellationToken = default);
}