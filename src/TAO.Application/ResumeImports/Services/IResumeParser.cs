using TAO.Application.ResumeImports.Models;
using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeImports.Services;

public interface IResumeParser
{
    Task<Result<ParsedResume>> ParseAsync(
        UploadedResume uploadedResume,
        CancellationToken cancellationToken);
}