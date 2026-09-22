using System.Text;
using TAO.AI.AssessmentStrategies.Contracts;
using TAO.Domain.ValueObjects;

namespace TAO.Application.AssessmentStrategies.Services;

internal sealed class AssessmentStrategyMarkdownGenerator
    : IAssessmentStrategyMarkdownGenerator
{
    public MarkdownContent Generate(
        AssessmentStrategyAiResponse structuredContent)
    {
        ArgumentNullException.ThrowIfNull(structuredContent);

        var builder = new StringBuilder();

        builder.AppendLine(
            $"# {structuredContent.AssessmentName}");

        builder.AppendLine();

        builder.AppendLine("## Assessment Overview");
        builder.AppendLine();

        builder.AppendLine(
            $"**Total Rounds:** {structuredContent.Rounds.Count}");

        builder.AppendLine(
            $"**Total Duration:** {structuredContent.Rounds.Sum(x => x.DurationInMinutes)} minutes");

        builder.AppendLine();

        builder.AppendLine("## Assessment Rounds");
        builder.AppendLine();

        foreach (var round in structuredContent.Rounds.OrderBy(x => x.Order))
        {
            AppendRound(
                builder,
                round);
        }

        return MarkdownContent.Create(
            builder.ToString().Trim());
    }

    private static void AppendRound(
        StringBuilder builder,
        AssessmentRoundAiResponse round)
    {
        builder.AppendLine(
            $"### Round {round.Order}: {round.Type}");

        builder.AppendLine();

        builder.AppendLine(
            $"- **Difficulty:** {round.Difficulty}");

        builder.AppendLine(
            $"- **Duration:** {round.DurationInMinutes} minutes");

        builder.AppendLine(
            $"- **Questions:** {round.QuestionCount}");

        builder.AppendLine();

        AppendCompetencies(
            builder,
            round.Competencies);
    }

    private static void AppendCompetencies(
        StringBuilder builder,
        IReadOnlyCollection<AssessmentCompetencyAiResponse> competencies)
    {
        builder.AppendLine("**Competencies:**");
        builder.AppendLine();

        if (competencies.Count == 0)
        {
            builder.AppendLine("- None");
        }
        else
        {
            foreach (var competency in competencies)
            {
                builder.AppendLine(
                    $"- **{competency.Name}:** {competency.Priority} priority");
            }
        }

        builder.AppendLine();
    }
}