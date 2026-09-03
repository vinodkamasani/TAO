
namespace TAO.Api.Endpoints.ResumeImports.Create;

public sealed class CreateResumeImportRequest
{
    public IReadOnlyList<IFormFile> Resumes { get; init; } = [];
}