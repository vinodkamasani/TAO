using TAO.AI.ResumeParsing.Contracts;
using TAO.SharedKernel.AI;
using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IResumeParserGenerator
{
    Task<Result<ResumeParsingResult>> ParseAsync(
        UploadedResume uploadedResume,
        CancellationToken cancellationToken = default);
}