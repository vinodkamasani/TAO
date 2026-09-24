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

        if (assessmentStrategy.Status ==
            AssessmentStrategyStatus.Approved)
        {
            return Result<AssessmentStrategyResponse>.Failure(
                Error.Validation(
                    "AssessmentStrategy.AlreadyApproved",
                    "An approved Assessment Strategy cannot be modified."));
        }

        var structuredResponse = new AssessmentStrategyAiResponse
        {
            AssessmentName = request.AssessmentName,
            Rounds = request.Rounds
                .Select(round => new AssessmentRoundAiResponse
                {
                    Order = round.Order,
                    Type = round.Type,
                    Difficulty = round.Difficulty,
                    DurationInMinutes = round.DurationInMinutes,
                    QuestionCount = round.QuestionCount,
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

        var structuredJson = JsonSerializer.Serialize(
            structuredResponse);

        var structuredContent = StructuredContent.Create(
            structuredJson);

        var markdownContent =
            markdownGenerator.Generate(
                structuredResponse);

        assessmentStrategy.Update(
            request.AssessmentName,
            markdownContent,
            structuredContent);

        await SynchronizeRoundsAsync(
            assessmentStrategy.Id,
            structuredResponse.Rounds,
            cancellationToken);

        await context.SaveChangesAsync(
            cancellationToken);

        var rounds = await context
            .Set<AssessmentRound>()
            .AsNoTracking()
            .Where(x =>
                x.AssessmentStrategyId ==
                assessmentStrategy.Id)
            .OrderBy(x => x.Order)
            .Select(x => new AssessmentRoundResponse(
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
            .ToListAsync(cancellationToken);

        return Result<AssessmentStrategyResponse>.Success(
     new AssessmentStrategyResponse(
         assessmentStrategy.Id,
         assessmentStrategy.OrganizationId,
         assessmentStrategy.CampaignId,
         assessmentStrategy.AssessmentName,
         assessmentStrategy.Content,
         assessmentStrategy.StructuredContent,
         assessmentStrategy.Status.ToString(),
         assessmentStrategy.GeneratedOn,
         rounds));
    }

    private async Task SynchronizeRoundsAsync(
        Guid assessmentStrategyId,
        IReadOnlyCollection<AssessmentRoundAiResponse> incomingRounds,
        CancellationToken cancellationToken)
    {
        var existingRounds = await context
            .Set<AssessmentRound>()
            .Include(x => x.Competencies)
            .Where(x =>
                x.AssessmentStrategyId == assessmentStrategyId)
            .ToListAsync(cancellationToken);

        var incomingOrders = incomingRounds
            .Select(x => x.Order)
            .ToHashSet();

        foreach (var existingRound in existingRounds)
        {
            if (!incomingOrders.Contains(existingRound.Order))
            {
                context
                    .Set<AssessmentRound>()
                    .Remove(existingRound);
            }
        }

        foreach (var incomingRound in incomingRounds)
        {
            var existingRound = existingRounds
                .FirstOrDefault(
                    x => x.Order == incomingRound.Order);

            var type = Enum.Parse<AssessmentRoundType>(
                incomingRound.Type,
                ignoreCase: true);

            var difficulty = Enum.Parse<AssessmentDifficulty>(
                incomingRound.Difficulty,
                ignoreCase: true);

            var competencies = incomingRound.Competencies
                .Select(x => new AssessmentRoundCompetency
                {
                    Name = x.Name,
                    Priority = x.Priority,
                    MinimumPassPercentage =
                        x.MinimumPassPercentage
                })
                .ToList();

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