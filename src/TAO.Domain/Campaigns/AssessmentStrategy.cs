

namespace TAO.Domain.Campaigns;

public sealed record AssessmentStrategy
{

    public AssessmentStrategy(
        AssessmentType assessmentType,
        int numberOfRounds,
        int durationInMinutes,
        DifficultyLevel difficulty,
        int passingScore
        )
    {
        if (numberOfRounds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfRounds),
                "Number of rounds must be greater than zero.");
        }

        if (durationInMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(durationInMinutes),
                "Duration must be greater than zero.");
        }

        if (passingScore < 0 || passingScore > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(passingScore),
                "Passing score must be between 0 and 100.");
        }

        AssessmentType = assessmentType;
        NumberOfRounds = numberOfRounds;
        DurationInMinutes = durationInMinutes;
        Difficulty = difficulty;
        PassingScore = passingScore;

    }
    public AssessmentType AssessmentType { get; }

    public int NumberOfRounds { get; }

    public int DurationInMinutes { get; }

    public DifficultyLevel Difficulty { get; }

    public int PassingScore { get; }




}
