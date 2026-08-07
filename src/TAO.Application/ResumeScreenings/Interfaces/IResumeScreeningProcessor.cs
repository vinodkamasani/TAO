using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeScreenings.Interfaces;

public interface IResumeScreeningProcessor
{
    Task<Result<Guid>> ProcessAsync(
        Guid candidateApplicationId,
        CancellationToken cancellationToken);
}