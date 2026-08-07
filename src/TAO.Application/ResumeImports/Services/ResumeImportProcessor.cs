using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.AI.ResumeParsing.Contracts;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeImports.Services;

internal sealed class ResumeImportProcessor : IResumeImportProcessor
{
    private readonly IApplicationDbContext _context;
    private readonly IResumeParserGenerator _resumeParserGenerator;

    public ResumeImportProcessor(
        IApplicationDbContext context,
        IResumeParserGenerator resumeParserGenerator)
    {
        _context = context;
        _resumeParserGenerator = resumeParserGenerator;
    }

    public async Task ProcessAsync(
    ResumeImport resumeImport,
    IReadOnlyCollection<UploadedResume> uploadedResumes,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resumeImport);
        ArgumentNullException.ThrowIfNull(uploadedResumes);

        resumeImport.MarkProcessing();

        foreach (var uploadedResume in uploadedResumes)
        {
            try
            {
                await ProcessResumeAsync(
                    resumeImport,
                    uploadedResume,
                    cancellationToken);

                resumeImport.RecordSuccess();
            }
            catch
            {
                resumeImport.RecordFailure();

                // TODO:
                // Log failure
                // Persist failure details for recruiter review
                // Continue processing remaining resumes
            }
        }

        resumeImport.Complete();

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessResumeAsync(
    ResumeImport resumeImport,
    UploadedResume uploadedResume,
    CancellationToken cancellationToken)
    {
        var parsingResult = await _resumeParserGenerator.ParseAsync(
            uploadedResume,
            cancellationToken);

        if (parsingResult.IsFailure)
        {
            throw new InvalidOperationException(
                parsingResult.Error.Message);
        }

        var application = await GetOrCreateCandidateApplicationAsync(
            resumeImport,
            parsingResult.Value,
            cancellationToken);

        await ReplaceResumeDataAsync(
            resumeImport,
            application,
            uploadedResume,
            parsingResult.Value,
            cancellationToken);
    }

    private async Task<CandidateApplication> GetOrCreateCandidateApplicationAsync(
    ResumeImport resumeImport,
    ResumeParsingResult parsingResult,
    CancellationToken cancellationToken)
    {

        var application = await _context.Set<CandidateApplication>()
            .FirstOrDefaultAsync(
                x => x.CampaignId == resumeImport.CampaignId &&
                     x.Email == parsingResult.Email,
                cancellationToken);

        if (application is not null)
        {
            return application;
        }

        application = CandidateApplication.Create(
            organizationId: resumeImport.OrganizationId,
            campaignId: resumeImport.CampaignId,
            candidateName: parsingResult.CandidateName,
            email: parsingResult.Email,
            phone: parsingResult.Phone ?? string.Empty,
            linkedInUrl: parsingResult.LinkedInUrl,
            currentCompany: parsingResult.CurrentCompany,
            currentLocation: parsingResult.CurrentLocation);

        _context.Set<CandidateApplication>().Add(application);

        return application;
    }

    private async Task ReplaceResumeDataAsync(
    ResumeImport resumeImport,
    CandidateApplication application,
    UploadedResume uploadedResume,
    ResumeParsingResult parsingResult,
    CancellationToken cancellationToken)
    {
        await ReplaceResumeAsync(
            resumeImport,
            application,
            uploadedResume,
            cancellationToken);

        await ReplaceResumeProfileAsync(
            resumeImport,
            application,
            parsingResult,
            cancellationToken);

        await ReplaceResumeScreeningAsync(
            resumeImport,
            application,
            parsingResult,
            cancellationToken);
    }


    private async Task ReplaceResumeAsync(
    ResumeImport resumeImport,
    CandidateApplication application,
    UploadedResume uploadedResume,
    CancellationToken cancellationToken)
    {
        var existingResume = await _context.Set<Resume>()
            .FirstOrDefaultAsync(
                x => x.ApplicationId == application.Id,
                cancellationToken);

        if (existingResume is not null)
        {
            _context.Set<Resume>().Remove(existingResume);
        }

        var resume = Resume.Create(
            organizationId: resumeImport.OrganizationId,
            applicationId: application.Id,
            fileName: uploadedResume.FileName,
            contentType: uploadedResume.ContentType,
            fileSize: uploadedResume.Content.LongLength,
            fileContent: uploadedResume.Content);

        _context.Set<Resume>().Add(resume);
    }

    private async Task ReplaceResumeProfileAsync(
    ResumeImport resumeImport,
    CandidateApplication application,
    ResumeParsingResult parsingResult,
    CancellationToken cancellationToken)
    {
        var existingProfile = await _context.Set<ResumeProfile>()
            .FirstOrDefaultAsync(
                x => x.ApplicationId == application.Id,
                cancellationToken);

        if (existingProfile is not null)
        {
            _context.Set<ResumeProfile>().Remove(existingProfile);
        }

        var profile = ResumeProfile.Create(
            organizationId: resumeImport.OrganizationId,
            applicationId: application.Id,
            structuredContent: StructuredContent.Create(
                parsingResult.StructuredContent));

        _context.Set<ResumeProfile>().Add(profile);
    }

    private async Task ReplaceResumeScreeningAsync(
    ResumeImport resumeImport,
    CandidateApplication application,
    ResumeParsingResult parsingResult,
    CancellationToken cancellationToken)
    {
        var existingScreening = await _context.Set<ResumeScreening>()
            .FirstOrDefaultAsync(
                x => x.ApplicationId == application.Id,
                cancellationToken);

        if (existingScreening is not null)
        {
            _context.Set<ResumeScreening>().Remove(existingScreening);
        }

        var screening = ResumeScreening.Create(
            organizationId: resumeImport.OrganizationId,
            applicationId: application.Id,
            content: MarkdownContent.Create(
                parsingResult.RawResponse),
            structuredContent: StructuredContent.Create(
                parsingResult.StructuredContent));

        _context.Set<ResumeScreening>().Add(screening);
    }

    public Task<Result> ProcessAsync(Guid resumeImportId, IReadOnlyCollection<UploadedResume> resumes, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}