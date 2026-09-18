using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TAO.AI.Abstractions;
using TAO.AI.AssessmentStrategies.Contracts;
using TAO.Application.AssessmentStrategies.Services;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Create;

internal sealed class CreateAssessmentStrategyCommandHandler
    : IRequestHandler<
        CreateAssessmentStrategyCommand,
        Result<CreateAssessmentStrategyResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAssessmentStrategyGenerator _assessmentStrategyGenerator;
    private readonly IAssessmentStrategyMarkdownGenerator _markdownGenerator;

    public CreateAssessmentStrategyCommandHandler(
        IApplicationDbContext context,
        IAssessmentStrategyGenerator assessmentStrategyGenerator,
        IAssessmentStrategyMarkdownGenerator assessmentStrategyMarkdownGenerator)
    {
        _context = context;
        _assessmentStrategyGenerator = assessmentStrategyGenerator;
        _markdownGenerator = assessmentStrategyMarkdownGenerator;
    }

    public async Task<Result<CreateAssessmentStrategyResponse>> Handle(
        CreateAssessmentStrategyCommand request,
        CancellationToken cancellationToken)
    {
        // ------------------------------------------------------------
        // Load Campaign
        // ------------------------------------------------------------

        var campaign = await _context
            .Set<Campaign>()
            .FirstOrDefaultAsync(
                c => c.Id == request.CampaignId,
                cancellationToken);

        if (campaign is null)
        {
            return Result<CreateAssessmentStrategyResponse>.Failure(
                Error.NotFound(
                    "Campaign.NotFound",
                    $"Campaign '{request.CampaignId}' was not found."));
        }

        // ------------------------------------------------------------
        // Load Job Profile
        // ------------------------------------------------------------

        var jobProfile = await _context
            .Set<JobProfile>()
            .FirstOrDefaultAsync(
                jp => jp.CampaignId == request.CampaignId,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result<CreateAssessmentStrategyResponse>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"No Job Profile found for Campaign '{request.CampaignId}'."));
        }

        // ------------------------------------------------------------
        // Job Profile must be approved
        // ------------------------------------------------------------

        if (jobProfile.Status != JobProfileStatus.Approved)
        {
            return Result<CreateAssessmentStrategyResponse>.Failure(
                Error.Validation(
                    "JobProfile.NotApproved",
                    "The Job Profile must be approved before generating an Assessment Strategy."));
        }

        // ------------------------------------------------------------
        // Load Hiring Strategy
        // ------------------------------------------------------------

        var hiringStrategy = await _context
            .Set<HiringStrategy>()
            .FirstOrDefaultAsync(
                hs => hs.CampaignId == request.CampaignId,
                cancellationToken);

        if (hiringStrategy is null)
        {
            return Result<CreateAssessmentStrategyResponse>.Failure(
                Error.NotFound(
                    "HiringStrategy.NotFound",
                    $"No Hiring Strategy found for Campaign '{request.CampaignId}'."));
        }

        // ------------------------------------------------------------
        // Hiring Strategy must be approved
        // ------------------------------------------------------------

        if (hiringStrategy.Status != HiringStrategyStatus.Approved)
        {
            return Result<CreateAssessmentStrategyResponse>.Failure(
                Error.Validation(
                    "HiringStrategy.NotApproved",
                    "The Hiring Strategy must be approved before generating an Assessment Strategy."));
        }

        // ------------------------------------------------------------
        // Check Existing Assessment Strategy
        // ------------------------------------------------------------

        var existingAssessmentStrategy = await _context
            .Set<AssessmentStrategy>()
            .AnyAsync(
                x => x.CampaignId == request.CampaignId,
                cancellationToken);

        if (existingAssessmentStrategy)
        {
            return Result<CreateAssessmentStrategyResponse>.Failure(
                Error.Conflict(
                    "AssessmentStrategy.AlreadyExists",
                    $"An Assessment Strategy already exists for Campaign '{request.CampaignId}'."));
        }

        // ------------------------------------------------------------
        // Generate Assessment Strategy
        // ------------------------------------------------------------

        var aiResult =
            await _assessmentStrategyGenerator.GenerateAsync(
                jobProfile,
                hiringStrategy,
                cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result<CreateAssessmentStrategyResponse>.Failure(
                aiResult.Error);
        }

        // ------------------------------------------------------------
        // Deserialize Structured AI Response
        // ------------------------------------------------------------

        var structuredResponse =
            JsonSerializer.Deserialize<AssessmentStrategyAiResponse>(
                aiResult.Value.StructuredContent.Value,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (structuredResponse is null)
        {
            return Result<CreateAssessmentStrategyResponse>.Failure(
                Error.Validation(
                    "AssessmentStrategy.InvalidResponse",
                    "The generated Assessment Strategy could not be processed."));
        }

        // ------------------------------------------------------------
        // Generate Markdown
        // ------------------------------------------------------------

        var markdownContent =
            _markdownGenerator.Generate(
                aiResult.Value);

        // ------------------------------------------------------------
        // Create Assessment Strategy
        // ------------------------------------------------------------

        var assessmentStrategy = new AssessmentStrategy(
            campaign.OrganizationId,
            campaign.Id,
            structuredResponse.AssessmentName,
            markdownContent,
            aiResult.Value.StructuredContent,
            aiResult.Value.Prompt,
            aiResult.Value.RawResponse,
            aiResult.Value.ProviderName,
            aiResult.Value.ModelName,
            aiResult.Value.PromptVersion);

        _context
            .Set<AssessmentStrategy>()
            .Add(assessmentStrategy);

        // ------------------------------------------------------------
        // Create Assessment Rounds
        // ------------------------------------------------------------

        var assessmentRounds = new List<AssessmentRound>();

        foreach (var round in structuredResponse.Rounds)
        {
            var assessmentRound = AssessmentRound.Create(
                assessmentStrategy.Id,
                round.Order,
                Enum.Parse<AssessmentRoundType>(
                    round.Type,
                    ignoreCase: true),
                Enum.Parse<AssessmentDifficulty>(
                    round.Difficulty,
                    ignoreCase: true),
                round.DurationInMinutes,
                round.QuestionCount,
                round.Competencies
                    .Select(x => new AssessmentRoundCompetency
                    {
                        Name = x.Name,
                        Priority = x.Priority,
                        MinimumPassPercentage =
                            x.MinimumPassPercentage
                    })
                    .ToList());

            _context
                .Set<AssessmentRound>()
                .Add(assessmentRound);

            assessmentRounds.Add(assessmentRound);
        }

        // ------------------------------------------------------------
        // Save
        // ------------------------------------------------------------

        await _context.SaveChangesAsync(
            cancellationToken);

        // ------------------------------------------------------------
        // Map to Response
        // ------------------------------------------------------------

        var response = new CreateAssessmentStrategyResponse(
            assessmentStrategy.Id,
            assessmentStrategy.OrganizationId,
            assessmentStrategy.CampaignId,
            assessmentStrategy.AssessmentName,
            assessmentStrategy.Content,
            assessmentStrategy.StructuredContent,
            assessmentStrategy.Status,
            assessmentStrategy.GeneratedOn,
            assessmentRounds
                .OrderBy(x => x.Order)
                .Select(x =>
                   new CreateAssessmentRoundResponse(
                    x.Id,
                    x.Order,
                    x.Type.ToString(),
                    x.Difficulty.ToString(),
                    x.DurationInMinutes,
                    x.TargetQuestionCount,
                    x.Competencies
                        .Select(c =>
                            new CreateAssessmentRoundCompetencyResponse(
                                c.Name,
                                c.Priority.ToString(),
                                c.MinimumPassPercentage))
                        .ToList()))
                .ToList());

        return Result<CreateAssessmentStrategyResponse>.Success(
            response);
    }
}