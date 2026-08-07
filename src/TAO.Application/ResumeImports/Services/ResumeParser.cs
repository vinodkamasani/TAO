using TAO.Application.ResumeImports.Models;
using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeImports.Services;

internal sealed class ResumeParser : IResumeParser
{
    public async Task<Result<ParsedResume>> ParseAsync(
        UploadedResume uploadedResume,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return Result<ParsedResume>.Failure(
            Error.Failure(
                "ResumeParser.NotImplemented",
                "Resume parsing has not been implemented."));
    }
}