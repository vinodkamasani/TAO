using TAO.Domain.Abstractions;
using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class AssessmentSession : AggregateRoot
{
    private AssessmentSession()
    {
    }

    private AssessmentSession(
        Guid candidateApplicationId,
        Guid assessmentStrategyId,
        AssessmentStrategySnapshot strategySnapshot,
        DateTime expiresOn)
    {
        CandidateApplicationId = Guard.AgainstEmpty(
            candidateApplicationId,
            nameof(CandidateApplicationId));

        AssessmentStrategyId = Guard.AgainstEmpty(
            assessmentStrategyId,
            nameof(AssessmentStrategyId));

        StrategySnapshot = strategySnapshot;

        AssessmentExpiresOn = expiresOn;
        Status = AssessmentSessionStatus.NotStarted;
        LastActivityOn = DateTime.UtcNow;
    }


    public Guid CandidateApplicationId { get; private set; }

    public Guid AssessmentStrategyId { get; private set; }

    public AssessmentStrategySnapshot StrategySnapshot { get; private set; } = null!;

    public AssessmentSessionStatus Status { get; private set; }

    public Guid? CurrentSessionRoundId { get; private set; }

    public Guid? CurrentQuestionId { get; private set; }

    public DateTime? ConsentAcceptedOn { get; private set; }

    public int ConsentVersion { get; private set; }

    public DateTime? StartedOn { get; private set; }

    public DateTime? CompletedOn { get; private set; }

    public DateTime AssessmentExpiresOn { get; private set; }

    public DateTime LastActivityOn { get; private set; }

    public bool HasUsedInterruptionWindow { get; private set; }
    public bool IsInterrupted { get; private set; }

    public static AssessmentSession Create(
        Guid candidateApplicationId,
        Guid assessmentStrategyId,
        AssessmentStrategySnapshot strategySnapshot,
        DateTime expiresOn)
    {
        ArgumentNullException.ThrowIfNull(strategySnapshot);

        if (expiresOn <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "Assessment session expiry must be in the future.",
                nameof(expiresOn));
        }

        return new AssessmentSession(
            candidateApplicationId,
            assessmentStrategyId,
            strategySnapshot,
            expiresOn);
    }

    public void AcceptConsent(int consentVersion)
    {
        EnsureStatus(AssessmentSessionStatus.NotStarted);

        if (consentVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(consentVersion),
                "Consent version must be greater than zero.");
        }

        ConsentAcceptedOn = DateTime.UtcNow;
        ConsentVersion = consentVersion;

        Touch();
    }

    public void Start()
    {
        EnsureStatus(AssessmentSessionStatus.NotStarted);

        if (!ConsentAcceptedOn.HasValue)
        {
            throw new InvalidOperationException(
                "Candidate must accept the consent before starting the assessment.");
        }

        if (IsExpired())
        {
            Status = AssessmentSessionStatus.Expired;
            throw new InvalidOperationException(
                "The assessment session has expired.");
        }

        Status = AssessmentSessionStatus.InProgress;
        StartedOn = DateTime.UtcNow;

        Touch();
    }

    public void SetCurrentRound(Guid sessionRoundId)
    {
        EnsureInProgress();

        CurrentSessionRoundId = Guard.AgainstEmpty(
            sessionRoundId,
            nameof(sessionRoundId));

        CurrentQuestionId = null;

        Touch();
    }

    public void SetCurrentQuestion(Guid questionId)
    {
        EnsureInProgress();

        CurrentQuestionId = Guard.AgainstEmpty(
            questionId,
            nameof(questionId));

        Touch();
    }

    public void RegisterInterruption()
    {
        EnsureInProgress();

        if (HasUsedInterruptionWindow)
        {
            Terminate();
            return;
        }

        HasUsedInterruptionWindow = true;
        IsInterrupted = true;

        Touch();
    }

    public void Resume()
    {
        EnsureInProgress();

        if (!IsInterrupted)
        {
            throw new InvalidOperationException(
                "Assessment session is not interrupted.");
        }

        if (IsExpired())
        {
            Terminate();
            return;
        }

        IsInterrupted = false;

        Touch();
    }

    public void Complete()
    {
        EnsureInProgress();

        Status = AssessmentSessionStatus.Completed;
        CompletedOn = DateTime.UtcNow;

        Touch();
    }

    public void Abandon()
    {
        EnsureInProgress();

        Status = AssessmentSessionStatus.Abandoned;

        Touch();
    }

    public void Expire()
    {
        if (Status is AssessmentSessionStatus.Completed
            or AssessmentSessionStatus.Abandoned
            or AssessmentSessionStatus.Terminated
            or AssessmentSessionStatus.Expired)
        {
            return;
        }

        Status = AssessmentSessionStatus.Expired;

        Touch();
    }

    public void Terminate()
    {
        if (Status is AssessmentSessionStatus.Completed
            or AssessmentSessionStatus.Abandoned
            or AssessmentSessionStatus.Terminated
            or AssessmentSessionStatus.Expired)
        {
            return;
        }

        Status = AssessmentSessionStatus.Terminated;

        Touch();
    }

    public void RecordActivity()
    {
        EnsureInProgress();

        Touch();
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= AssessmentExpiresOn;
    }

    private void EnsureInProgress()
    {
        if (Status != AssessmentSessionStatus.InProgress)
        {
            throw new InvalidOperationException(
                $"Assessment session must be in progress. Current status: {Status}.");
        }
    }

    private void EnsureStatus(AssessmentSessionStatus expectedStatus)
    {
        if (Status != expectedStatus)
        {
            throw new InvalidOperationException(
                $"Assessment session must be in {expectedStatus} status. " +
                $"Current status: {Status}.");
        }
    }

    private void Touch()
    {
        LastActivityOn = DateTime.UtcNow;
    }
}