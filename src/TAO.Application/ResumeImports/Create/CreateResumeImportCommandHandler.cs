using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.Application.AssessmentQuestions.Generate;
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
    private readonly ICurrentUser _currentUser;

    public CreateResumeImportCommandHandler(
        IApplicationDbContext context,
        IResumeImportProcessor resumeImportProcessor,
        ICurrentUser currentUser)
    {
        _context = context;
        _resumeImportProcessor = resumeImportProcessor;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateResumeImportCommand request,
        CancellationToken cancellationToken)
    {

        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!_currentUser.IsAuthenticated)
        {
            return Result<Guid>.Failure(
                Error.Unauthorized(
                    "CreateResumeImport.Unauthorized",
                    "The current user is not authenticated."));
        }

        // ---------------------------------------------------------
        // 2. Get organization from authenticated user
        // ---------------------------------------------------------

        var organizationId = _currentUser.OrganizationId;

        if (organizationId is null)
        {
            return Result<Guid>.Failure(
                Error.Unauthorized(
                    "CreateResumeImport.OrganizationNotFound",
                    "The current user's organization could not be identified."));
        }

        // ------------------------------------------------------------------
        // Validate Campaign
        // ------------------------------------------------------------------

        var campaign = await _context
            .Set<Campaign>()
            .FirstOrDefaultAsync(
                c => c.Id == request.CampaignId && c.OrganizationId == organizationId.Value,
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