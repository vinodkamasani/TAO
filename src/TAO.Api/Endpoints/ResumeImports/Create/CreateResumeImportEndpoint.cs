using MediatR;
using Microsoft.AspNetCore.Mvc;
using TAO.Api.Extensions;
using TAO.Application.ResumeImports.Create;
using TAO.Application.ResumeImports.Models;
using TAO.SharedKernel.AI.Models;

namespace TAO.Api.Endpoints.ResumeImports.Create;

public static class CreateResumeImportEndpoint
{
    public static RouteGroupBuilder MapCreateResumeImportEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost(
                "/{campaignId:guid}/resume-imports",
                HandleAsync)
            .WithName("CreateResumeImport")
            .WithSummary("Imports resumes into a campaign.")
            .WithDescription(
                "Uploads one or more resumes and starts the resume import process.")
             .Accepts<CreateResumeImportRequest>("multipart/form-data")
            .DisableAntiforgery();

        return group;
    }

    private static async Task<IResult> HandleAsync(
    Guid campaignId,
    [FromForm] CreateResumeImportRequest request,
    ISender sender,
    CancellationToken cancellationToken)
    {
        if (request?.Resumes == null || request.Resumes.Count == 0)
        {
            return Results.BadRequest("At least one resume must be uploaded.");
        }

        var uploadedResumes = new List<UploadedResume>();

        foreach (var file in request.Resumes)
        {
            await using var stream = file.OpenReadStream();

            using var memory = new MemoryStream();

            await stream.CopyToAsync(
                memory,
                cancellationToken);

            uploadedResumes.Add(
                new UploadedResume(
                    file.FileName,
                    file.ContentType,
                    memory.ToArray()));
        }

        var command = new CreateResumeImportCommand(
            campaignId,
            uploadedResumes);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToCreatedResult(
            $"/api/resumeimports/{result.Value}");
    }
}