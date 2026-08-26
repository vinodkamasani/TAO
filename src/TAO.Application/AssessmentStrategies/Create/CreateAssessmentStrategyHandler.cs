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
    : IRequestHandler<CreateAssessmentStrategyCommand, Result<Guid>>
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

    public async Task<Result<Guid>> Handle(
        CreateAssessmentStrategyCommand request,
        CancellationToken cancellationToken)
    {
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

        var jobProfile = await _context
            .Set<JobProfile>()
            .FirstOrDefaultAsync(
                jp => jp.CampaignId == request.CampaignId,
                cancellationToken);

        if (jobProfile is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "JobProfile.NotFound",
                    $"No Job Profile found for Campaign '{request.CampaignId}'."));
        }

        if (jobProfile.Status != JobProfileStatus.Approved)
        {
            return Result<Guid>.Failure(
                Error.Validation(
                    "JobProfile.NotApproved",
                    "The Job Profile must be approved before generating an Assessment Strategy."));
        }

        var hiringStrategy = await _context
            .Set<HiringStrategy>()
            .FirstOrDefaultAsync(
                hs => hs.CampaignId == request.CampaignId,
                cancellationToken);

        if (hiringStrategy is null)
        {
            return Result<Guid>.Failure(
                Error.NotFound(
                    "HiringStrategy.NotFound",
                    $"No Hiring Strategy found for Campaign '{request.CampaignId}'."));
        }

        if (hiringStrategy.Status != HiringStrategyStatus.Approved)
        {
            return Result<Guid>.Failure(
                Error.Validation(
                    "HiringStrategy.NotApproved",
                    "The Hiring Strategy must be approved before generating an Assessment Strategy."));
        }

        var existingAssessmentStrategy = await _context
          .Set<AssessmentStrategy>()
          .AnyAsync(
              x => x.CampaignId == request.CampaignId,
              cancellationToken);

        if (existingAssessmentStrategy)
        {
            return Result<Guid>.Failure(
                Error.Conflict(
                    "AssessmentStrategy.AlreadyExists",
                    $"An Assessment Strategy already exists for Campaign '{request.CampaignId}'."));
        }

        var aiResult = await _assessmentStrategyGenerator.GenerateAsync(
            jobProfile,
            hiringStrategy,
            cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result<Guid>.Failure(aiResult.Error);
        }

      

        var structuredResponse =
            JsonSerializer.Deserialize<AssessmentStrategyAiResponse>(
                aiResult.Value.StructuredContent.Value,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (structuredResponse is null)
        {
            return Result<Guid>.Failure(
                Error.Validation(
                    "AssessmentStrategy.InvalidResponse",
                    "The generated Assessment Strategy could not be processed."));
        }

        var markdownContent =
                  _markdownGenerator.Generate(
                      aiResult.Value);

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

        foreach (var round in structuredResponse.Rounds)
        {
            var assessmentRound = AssessmentRound.Create(
                assessmentStrategy.Id,
                round.Order,
                Enum.Parse<AssessmentRoundType>(round.Type, ignoreCase: true),
                Enum.Parse<AssessmentDifficulty>(round.Difficulty, ignoreCase: true),
                round.DurationInMinutes,
                round.QuestionCount,
                round.Competencies
                    .Select(x => new AssessmentRoundCompetency
                    {
                        Name = x.Name,
                        Priority = x.Priority,
                        MinimumPassPercentage = x.MinimumPassPercentage
                    })
                    .ToList()); // List<T> implements IReadOnlyCollection<T>

            _context
                .Set<AssessmentRound>()
                .Add(assessmentRound);
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            assessmentStrategy.Id);
    }
}