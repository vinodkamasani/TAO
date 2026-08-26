using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class AssessmentSessionRound : Entity
{
    private readonly List<AssessmentRoundCompetency> _competencies = [];

    private AssessmentSessionRound()
    {
    }

    private AssessmentSessionRound(
        Guid assessmentSessionId,
        Guid assessmentRoundId,
        int order,
        AssessmentRoundType type,
        AssessmentDifficulty difficulty,
        int durationInMinutes,
        int targetQuestionCount,
        IReadOnlyCollection<AssessmentRoundCompetency> competencies)
    {
        AssessmentSessionId = Guard.AgainstEmpty(
            assessmentSessionId,
            nameof(AssessmentSessionId));

        AssessmentRoundId = Guard.AgainstEmpty(
            assessmentRoundId,
            nameof(AssessmentRoundId));

        if (order <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(order),
                "Round order must be greater than zero.");
        }

        if (durationInMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(durationInMinutes),
                "Round duration must be greater than zero.");
        }

        if (targetQuestionCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(targetQuestionCount),
                "Target question count must be greater than zero.");
        }

        ArgumentNullException.ThrowIfNull(competencies);

        Order = order;
        Type = type;
        Difficulty = difficulty;
        DurationInMinutes = durationInMinutes;
        TargetQuestionCount = targetQuestionCount;

        _competencies.AddRange(competencies);

        Status = AssessmentSessionRoundStatus.NotStarted;
    }

    public Guid AssessmentSessionId { get; private set; }

    public Guid AssessmentRoundId { get; private set; }

    public int Order { get; private set; }

    public AssessmentRoundType Type { get; private set; }

    public AssessmentDifficulty Difficulty { get; private set; }

    public int DurationInMinutes { get; private set; }

    public int TargetQuestionCount { get; private set; }

    public AssessmentSessionRoundStatus Status { get; private set; }

    public DateTime? StartedOn { get; private set; }

    public DateTime? ExpiresOn { get; private set; }

    public DateTime? CompletedOn { get; private set; }

    public IReadOnlyCollection<AssessmentRoundCompetency> Competencies =>
        _competencies.AsReadOnly();

    public ICollection<AssessmentQuestion> Questions { get; } = [];

    public static AssessmentSessionRound Create(
        Guid assessmentSessionId,
        AssessmentRound assessmentRound)
    {
        ArgumentNullException.ThrowIfNull(assessmentRound);

        return new AssessmentSessionRound(
            assessmentSessionId,
            assessmentRound.Id,
            assessmentRound.Order,
            assessmentRound.Type,
            assessmentRound.Difficulty,
            assessmentRound.DurationInMinutes,
            assessmentRound.TargetQuestionCount,
            assessmentRound.Competencies);
    }

    public void Start(DateTime startedOn, DateTime expiresOn)
    {
        if (Status != AssessmentSessionRoundStatus.NotStarted)
        {
            throw new InvalidOperationException(
                "Only a not started round can be started.");
        }

        if (expiresOn <= startedOn)
        {
            throw new ArgumentException(
                "Round expiration must be after the start time.",
                nameof(expiresOn));
        }

        StartedOn = startedOn;
        ExpiresOn = expiresOn;
        Status = AssessmentSessionRoundStatus.InProgress;
    }

    public void Complete(DateTime completedOn)
    {
        if (Status != AssessmentSessionRoundStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Only an in-progress round can be completed.");
        }

        CompletedOn = completedOn;
        Status = AssessmentSessionRoundStatus.Completed;
    }

    public void Expire(DateTime expiredOn)
    {
        if (Status != AssessmentSessionRoundStatus.InProgress)
        {
            return;
        }

        CompletedOn = expiredOn;
        Status = AssessmentSessionRoundStatus.Expired;
    }

    public void Terminate(DateTime terminatedOn)
    {
        if (Status == AssessmentSessionRoundStatus.Completed)
        {
            throw new InvalidOperationException(
                "A completed round cannot be terminated.");
        }

        CompletedOn = terminatedOn;
        Status = AssessmentSessionRoundStatus.Terminated;
    }
}