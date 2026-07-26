using TAO.AI.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface ILLMProvider
{
    Task<Result<LLMResponse>> GenerateAsync(
        LLMRequest request,
        CancellationToken cancellationToken);
}