using TAO.Domain.Common;
using TAO.Domain.Enums;

namespace TAO.Domain.Entities;

public sealed class AssessmentResult : Entity
{
    private AssessmentResult()
    {
    }

    private AssessmentResult(
        Guid assessmentSessionId,
        byte overallScore,
        byte overallConfidence,
        AssessmentRecommendation recommendation,
        string executiveSummary)
    {
        AssessmentSessionId = Guard.AgainstEmpty(
            assessmentSessionId,
            nameof(AssessmentSessionId));

        ValidatePercentage(
            overallScore,
            nameof(overallScore));

        ValidatePercentage(
            overallConfidence,
            nameof(overallConfidence));

        OverallScore = overallScore;
        OverallConfidence = overallConfidence;
        Recommendation = recommendation;

        ExecutiveSummary = Guard.AgainstNullOrWhiteSpace(
            executiveSummary,
            nameof(ExecutiveSummary));

        GeneratedOn = DateTime.UtcNow;
    }

    public Guid AssessmentSessionId { get; private set; }

    public byte OverallScore { get; private set; }

    public byte OverallConfidence { get; private set; }

    public AssessmentRecommendation Recommendation { get; private set; }

    public string ExecutiveSummary { get; private set; } = null!;

    public DateTime GeneratedOn { get; private set; }

    public Guid? ReviewedByUserId { get; private set; }

    public DateTime? ReviewedOn { get; private set; }

    public RecruiterAssessmentDecision? RecruiterDecision { get; private set; }

    public string? RecruiterComments { get; private set; }

    public static AssessmentResult Create(
        Guid assessmentSessionId,
        byte overallScore,
        byte overallConfidence,
        AssessmentRecommendation recommendation,
        string executiveSummary)
    {
        return new AssessmentResult(
            assessmentSessionId,
            overallScore,
            overallConfidence,
            recommendation,
            executiveSummary);
    }

    public void RecordRecruiterDecision(
        Guid reviewedByUserId,
        RecruiterAssessmentDecision decision,
        string? comments = null)
    {
        ReviewedByUserId = Guard.AgainstEmpty(
            reviewedByUserId,
            nameof(reviewedByUserId));

        RecruiterDecision = decision;

        RecruiterComments = comments;

        ReviewedOn = DateTime.UtcNow;
    }

    private static void ValidatePercentage(
        byte value,
        string parameterName)
    {
        if (value > 100)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Percentage must be between 0 and 100.");
        }
    }
}