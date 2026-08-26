using TAO.Domain.Common;
using TAO.Domain.Enums;
using TAO.Domain.ValueObjects;

namespace TAO.Domain.Entities;

public sealed class AssessmentQuestion : Entity
{
    private AssessmentQuestion()
    {
    }

    private AssessmentQuestion(
        Guid assessmentSessionRoundId,
        int order,
        string primaryQuestion,
        IReadOnlyCollection<string> competencies)
    {
        AssessmentSessionRoundId = Guard.AgainstEmpty(
            assessmentSessionRoundId,
            nameof(AssessmentSessionRoundId));

        if (order <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(order),
                "Question order must be greater than zero.");
        }

        PrimaryQuestion = Guard.AgainstNullOrWhiteSpace(
            primaryQuestion,
            nameof(PrimaryQuestion));

        ArgumentNullException.ThrowIfNull(competencies);

        Order = order;

        _competencies.AddRange(
            competencies
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()));

        Status = AssessmentQuestionStatus.NotStarted;
    }

    private readonly List<string> _competencies = [];

    public Guid AssessmentSessionRoundId { get; private set; }

    public int Order { get; private set; }

    public string PrimaryQuestion { get; private set; } = null!;

    public AssessmentQuestionStatus Status { get; private set; }

    public ConversationContent? Conversation { get; private set; }

    public string? CandidateCode { get; private set; }

    public DateTime? StartedOn { get; private set; }

    public DateTime? CompletedOn { get; private set; }

    public IReadOnlyCollection<string> Competencies =>
        _competencies.AsReadOnly();

    public static AssessmentQuestion Create(
        Guid assessmentSessionRoundId,
        int order,
        string primaryQuestion,
        IReadOnlyCollection<string> competencies)
    {
        return new AssessmentQuestion(
            assessmentSessionRoundId,
            order,
            primaryQuestion,
            competencies);
    }

    public void Start(DateTime startedOn)
    {
        if (Status != AssessmentQuestionStatus.NotStarted)
        {
            throw new InvalidOperationException(
                "Only a not started question can be started.");
        }

        StartedOn = startedOn;
        Status = AssessmentQuestionStatus.InProgress;
    }

    public void UpdateConversation(
        ConversationContent conversation)
    {
        ArgumentNullException.ThrowIfNull(conversation);

        Conversation = conversation;
    }

    public void SetCandidateCode(string? candidateCode)
    {
        CandidateCode = candidateCode;
    }

    public void Skip(DateTime completedOn)
    {
        if (Status != AssessmentQuestionStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Only an in-progress question can be skipped.");
        }

        CompletedOn = completedOn;
        Status = AssessmentQuestionStatus.Skipped;
    }

    public void Complete(DateTime completedOn)
    {
        if (Status != AssessmentQuestionStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Only an in-progress question can be completed.");
        }

        CompletedOn = completedOn;
        Status = AssessmentQuestionStatus.Completed;
    }
}