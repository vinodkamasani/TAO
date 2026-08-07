using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IDocumentTextExtractor
{
    Task<Result<string>> ExtractTextAsync(
        UploadedResume uploadedResume,
        CancellationToken cancellationToken = default);
}