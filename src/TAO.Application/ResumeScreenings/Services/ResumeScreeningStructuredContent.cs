namespace TAO.Application.ResumeScreenings.Services;

internal sealed class ResumeScreeningStructuredContent
{
    public required byte MandatorySkillsScore { get; init; }

    public required byte PreferredSkillsScore { get; init; }

    public required byte ExperienceScore { get; init; }

    public required byte ResponsibilitiesScore { get; init; }

    public required byte EducationScore { get; init; }

    public required byte DomainScore { get; init; }

    public required string ExecutiveSummary { get; init; }

    public required List<string> Strengths { get; init; }

    public required List<string> Gaps { get; init; }
    public required List<EvidenceItem> Evidence { get; init; }
}