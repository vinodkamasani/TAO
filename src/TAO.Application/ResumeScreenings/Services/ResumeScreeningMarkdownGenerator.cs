using System.Text;
using System.Text.Json;
using TAO.AI.ResumeScreening.Contracts;
using TAO.Domain.ValueObjects;

namespace TAO.Application.ResumeScreenings.Services;

internal sealed class ResumeScreeningMarkdownGenerator
    : IResumeScreeningMarkdownGenerator
{
    public MarkdownContent Generate(
        ResumeScreeningResult screeningResult)
    {
        ArgumentNullException.ThrowIfNull(screeningResult);

        var structuredContent =
            JsonSerializer.Deserialize<ResumeScreeningStructuredContent>(
                screeningResult.StructuredContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive=true
                })
            ?? throw new InvalidOperationException(
                "Unable to deserialize Resume Screening structured content.");

        var builder = new StringBuilder();

        builder.AppendLine("# Resume Screening");
        builder.AppendLine();

        builder.AppendLine($"**Overall Match:** {screeningResult.OverallMatchPercentage}%");
        builder.AppendLine();

        builder.AppendLine($"**Recommendation:** {(screeningResult.IsRecommended ? "Recommended" : "Not Recommended")}");
        builder.AppendLine();

        builder.AppendLine("## Executive Summary");
        builder.AppendLine();

        builder.AppendLine(structuredContent.ExecutiveSummary);
        builder.AppendLine();

        AppendSection(
            builder,
            "Strengths",
            structuredContent.Strengths);

        AppendSection(
            builder,
            "Gaps",
            structuredContent.Gaps);

        AppendEvidence(
             builder,
             structuredContent.Evidence);

        builder.AppendLine("## Score Breakdown");
        builder.AppendLine();

        AppendScore(builder, "Mandatory Skills", structuredContent.MandatorySkillsScore);
        AppendScore(builder, "Preferred Skills", structuredContent.PreferredSkillsScore);
        AppendScore(builder, "Relevant Experience", structuredContent.ExperienceScore);
        AppendScore(builder, "Responsibilities", structuredContent.ResponsibilitiesScore);
        AppendScore(builder, "Education", structuredContent.EducationScore);
        AppendScore(builder, "Domain Experience", structuredContent.DomainScore);

        builder.AppendLine();

        return MarkdownContent.Create(
            builder.ToString().Trim());
    }

    private static void AppendSection(
        StringBuilder builder,
        string heading,
        IReadOnlyCollection<string> items)
    {
        builder.AppendLine($"## {heading}");
        builder.AppendLine();

        if (items.Count == 0)
        {
            builder.AppendLine("- None");
        }
        else
        {
            foreach (var item in items)
            {
                builder.AppendLine($"- {item}");
            }
        }

        builder.AppendLine();
    }

    private static void AppendScore(
    StringBuilder builder,
    string label,
    byte score)
    {
        builder.AppendLine($"- **{label}:** {score}%");
    }

    private static void AppendEvidence(
    StringBuilder builder,
    IReadOnlyCollection<EvidenceItem> evidence)
    {
        builder.AppendLine("## Evidence");
        builder.AppendLine();

        if (evidence.Count == 0)
        {
            builder.AppendLine("- None");
        }
        else
        {
            foreach (var item in evidence)
            {
                builder.AppendLine(
                    $"- **{item.Requirement}:** {item.ResumeEvidence}");
            }
        }

        builder.AppendLine();
    }

}