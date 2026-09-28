using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TAO.AI.AssessmentStrategies.Contracts;
using TAO.Application.AssessmentStrategies.Contracts;
using TAO.Application.AssessmentStrategies.Services;
using TAO.Application.Common.Interfaces;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentStrategies.Update;

public sealed class UpdateAssessmentStrategyCommandHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser,
    IAssessmentStrategyMarkdownGenerator markdownGenerator)
    : IRequestHandler<
        UpdateAssessmentStrategyCommand,
        Result<AssessmentStrategyResponse>>
{
    public async Task<Result<AssessmentStrategyResponse>> Handle(
        UpdateAssessmentStrategyCommand request,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // 1. Validate authentication
        // ---------------------------------------------------------

        if (!currentUser.IsAuthenticated ||
            currentUser.UserId is null ||
            currentUser.OrganizationId is null)
        {
            return Result<AssessmentStrategyResponse>.Failure(
                Error.Unauthorized(
                    "AssessmentStrategy.Unauthorized",
                    "The current user is not authenticated."));
        }

        var organizationId = currentUser.OrganizationId.Value;

        // ---------------------------------------------------------
        // 2. Load Assessment Strategy with tenant isolation
        // ---------------------------------------------------------

        var assessmentStrategy = await context
            .Set<AssessmentStrategy>()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.AssessmentStrategyId &&
                    x.OrganizationId == organizationId,
                cancellationToken);

        if (assessmentStrategy is null)
        {
            return Result<AssessmentStrategyResponse>.Failure(
                Error.NotFound(
                    "AssessmentStrategy.NotFound",
                    "The Assessment Strategy was not found."));
        }

        // ---------------------------------------------------------
        // 3. Approved strategies cannot be modified
        // ---------------------------------------------------------

        if (assessmentStrategy.Status ==
            AssessmentStrategyStatus.Approved)
        {
            return Result<AssessmentStrategyResponse>.Failure(
                Error.Validation(
                    "AssessmentStrategy.AlreadyApproved",
                    "An approved Assessment Strategy cannot be modified."));
        }

        // ---------------------------------------------------------
        // 4. Build structured content from request
        //
        // Structured content is the canonical editable state.
        // Markdown is regenerated from it.
        // ---------------------------------------------------------

        var structuredResponse = new AssessmentStrategyAiResponse
        {
            AssessmentName = request.AssessmentName,

            Rounds = request.Rounds
                .Select(round =>
                    new AssessmentRoundAiResponse
                    {
                        Order = round.Order,
                        Type = round.Type,
                        Difficulty = round.Difficulty,
                        DurationInMinutes =
                            round.DurationInMinutes,
                        QuestionCount =
                            round.QuestionCount,

                        Competencies = round.Competencies
                            .Select(competency =>
                                new AssessmentCompetencyAiResponse
                                {
                                    Name = competency.Name,
                                    Priority = competency.Priority,
                                    MinimumPassPercentage =
                                        competency.MinimumPassPercentage
                                })
                            .ToList()
                    })
                .ToList()
        };

        // ---------------------------------------------------------
        // 5. Serialize structured content
        // ---------------------------------------------------------

        var structuredJson = JsonSerializer.Serialize(
            structuredResponse);

        var structuredContent = StructuredContent.Create(
            structuredJson);

        // ---------------------------------------------------------
        // 6. Regenerate Markdown
        // ---------------------------------------------------------

        var markdownContent =
            markdownGenerator.Generate(
                structuredResponse);

        // ---------------------------------------------------------
        // 7. Update Assessment Strategy
        // ---------------------------------------------------------

        assessmentStrategy.Update(
            request.AssessmentName,
            markdownContent,
            structuredContent);

        // ---------------------------------------------------------
        // 8. Synchronize Assessment Rounds
        // ---------------------------------------------------------

        await SynchronizeRoundsAsync(
            assessmentStrategy.Id,
            structuredResponse.Rounds,
            cancellationToken);

        // ---------------------------------------------------------
        // 9. Persist changes
        // ---------------------------------------------------------

        await context.SaveChangesAsync(
            cancellationToken);

        // ---------------------------------------------------------
        // 10. Reload rounds
        //
        // IMPORTANT:
        // Do not Include Competencies.
        //
        // Competencies is a JSON/value collection and is
        // automatically materialized with AssessmentRound.
        // ---------------------------------------------------------

        var rounds = await context
            .Set<AssessmentRound>()
            .AsNoTracking()
            .Where(x =>
                x.AssessmentStrategyId ==
                assessmentStrategy.Id)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // 11. Map rounds in memory
        //
        // This avoids EF Core trying to translate:
        //
        // x.Competencies.Select(...)
        //
        // and enum conversions such as ToString().
        // ---------------------------------------------------------

        var roundResponses = rounds
            .Select(x =>
                new AssessmentRoundResponse(
                    x.Id,
                    x.Order,
                    x.Type.ToString(),
                    x.Difficulty.ToString(),
                    x.DurationInMinutes,
                    x.TargetQuestionCount,
                    x.Competencies
                        .Select(c =>
                            new AssessmentCompetencyResponse(
                                c.Name,
                                c.Priority,
                                c.MinimumPassPercentage))
                        .ToList()))
            .ToList();

        // ---------------------------------------------------------
        // 12. Build response
        // ---------------------------------------------------------

        var response = new AssessmentStrategyResponse(
            assessmentStrategy.Id,
            assessmentStrategy.OrganizationId,
            assessmentStrategy.CampaignId,
            assessmentStrategy.AssessmentName,
            assessmentStrategy.Content,
            assessmentStrategy.StructuredContent,
            assessmentStrategy.Status.ToString(),
            assessmentStrategy.GeneratedOn,
            roundResponses);

        // ---------------------------------------------------------
        // 13. Return response
        // ---------------------------------------------------------

        return Result<AssessmentStrategyResponse>.Success(
            response);
    }

    private async Task SynchronizeRoundsAsync(
        Guid assessmentStrategyId,
        IReadOnlyCollection<AssessmentRoundAiResponse> incomingRounds,
        CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------
        // Load existing rounds.
        //
        // DO NOT use Include(x => x.Competencies).
        //
        // Competencies is stored as JSON and is automatically
        // loaded with the AssessmentRound entity.
        // ---------------------------------------------------------

        var existingRounds = await context
            .Set<AssessmentRound>()
            .Where(x =>
                x.AssessmentStrategyId ==
                assessmentStrategyId)
            .ToListAsync(cancellationToken);

        // ---------------------------------------------------------
        // Determine incoming round orders
        // ---------------------------------------------------------

        var incomingOrders = incomingRounds
            .Select(x => x.Order)
            .ToHashSet();

        // ---------------------------------------------------------
        // Remove rounds no longer present in the request
        // ---------------------------------------------------------

        foreach (var existingRound in existingRounds)
        {
            if (!incomingOrders.Contains(
                    existingRound.Order))
            {
                context
                    .Set<AssessmentRound>()
                    .Remove(existingRound);
            }
        }

        // ---------------------------------------------------------
        // Add or update rounds
        // ---------------------------------------------------------

        foreach (var incomingRound in incomingRounds)
        {
            var existingRound = existingRounds
                .FirstOrDefault(
                    x => x.Order == incomingRound.Order);

            // -----------------------------------------------------
            // Parse Type
            // -----------------------------------------------------

            if (!EnumExtensions.TryParseNormalized(
                    incomingRound.Type,
                    out AssessmentRoundType type))
            {
                throw new InvalidOperationException(
                    $"Invalid assessment round type '{incomingRound.Type}'.");
            }

            // -----------------------------------------------------
            // Parse Difficulty
            // -----------------------------------------------------

            if (!Enum.TryParse<AssessmentDifficulty>(
                    incomingRound.Difficulty,
                    ignoreCase: true,
                    out var difficulty))
            {
                throw new InvalidOperationException(
                    $"Invalid assessment difficulty '{incomingRound.Difficulty}'.");
            }

            // -----------------------------------------------------
            // Build competencies
            // -----------------------------------------------------

            var competencies = incomingRound.Competencies
                .Select(x =>
                    new AssessmentRoundCompetency
                    {
                        Name = x.Name,
                        Priority = x.Priority,
                        MinimumPassPercentage =
                            x.MinimumPassPercentage
                    })
                .ToList();

            // -----------------------------------------------------
            // Create new round
            // -----------------------------------------------------

            if (existingRound is null)
            {
                var newRound = AssessmentRound.Create(
                    assessmentStrategyId,
                    incomingRound.Order,
                    type,
                    difficulty,
                    incomingRound.DurationInMinutes,
                    incomingRound.QuestionCount,
                    competencies);

                context
                    .Set<AssessmentRound>()
                    .Add(newRound);

                continue;
            }

            // -----------------------------------------------------
            // Update existing round
            // -----------------------------------------------------

            existingRound.Update(
                incomingRound.Order,
                type,
                difficulty,
                incomingRound.DurationInMinutes,
                incomingRound.QuestionCount);

            existingRound.ReplaceCompetencies(
                competencies);
        }
    }
}