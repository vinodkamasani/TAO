using MediatR;
using TAO.Application.ResumeImports.Models;
using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeImports.Create;

public sealed record CreateResumeImportCommand(
    Guid CampaignId,
    IReadOnlyCollection<UploadedResume> Resumes)
    : IRequest<Result<Guid>>;