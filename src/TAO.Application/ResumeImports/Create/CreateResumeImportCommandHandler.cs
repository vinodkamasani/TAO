using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.Common.Interfaces;
using TAO.Application.ResumeImports.Services;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeImports.Create;

internal sealed class CreateResumeImportCommandHandler
    : IRequestHandler<CreateResumeImportCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IResumeImportProcessor _resumeImportProcessor;

    public CreateResumeImportCommandHandler(
        IApplicationDbContext context,
        IResumeImportProcessor resumeImportProcessor)
    {
        _context = context;
        _resumeImportProcessor = resumeImportProcessor;
    }

    public async Task<Result<Guid>> Handle(
        CreateResumeImportCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------------
        // Validate Campaign
        // ------------------------------------------------------------------

        var campaign = await _context
            .Set<Campaign>()
            .FirstOrDefaultAsync(
                c => c.Id == request.CampaignId,
                cancellationToken);

        if (campaign is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign '{request.CampaignId}' was not found."));
        }

        // ------------------------------------------------------------------
        // Create Resume Import
        // ------------------------------------------------------------------

        var resumeImport = ResumeImport.Create(
            campaign.OrganizationId,
            campaign.Id,
            request.Resumes.Count);

        _context
            .Set<ResumeImport>()
            .Add(resumeImport);

        await _context.SaveChangesAsync(cancellationToken);

        // ------------------------------------------------------------------
        // Process uploaded resumes
        // ------------------------------------------------------------------

       
            await _resumeImportProcessor.ProcessAsync(
                resumeImport,
                request.Resumes,
                cancellationToken);

        //if (processingResult.IsFailure)
        //{
        //    return Result<Guid>.Failure(
        //        processingResult.Error);
        //}

        return Result<Guid>.Success(resumeImport.Id);
    }
}