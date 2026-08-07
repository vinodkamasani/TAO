using TAO.Application.ResumeImports.Models;
using TAO.Domain.Entities;
using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeImports.Services;

public interface IResumeImportProcessor
{
    Task ProcessAsync(
        ResumeImport resumeImport,
        IReadOnlyCollection<UploadedResume> uploadedResumes,
        CancellationToken cancellationToken = default);
}