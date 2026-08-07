using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TAO.Api.Endpoints.ResumeImports.Create;

public sealed class CreateResumeImportRequest
{
    [FromForm]
    public List<IFormFile> Resumes { get; init; } = [];
}