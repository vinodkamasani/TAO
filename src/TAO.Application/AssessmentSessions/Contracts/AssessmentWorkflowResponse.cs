namespace TAO.Application.AssessmentSessions.Contracts;

public sealed record AssessmentWorkflowResponse(
    Guid AssessmentSessionId,
    string Status,
    string CurrentStage,
    int CompletionPercentage,

    int TotalRounds,
    int CompletedRounds,
    int RemainingRounds,

    int TotalQuestions,
    int CompletedQuestions,
    int SkippedQuestions,
    int RemainingQuestions,

    Guid? CurrentRoundId,
    int? CurrentRoundOrder,
    string? CurrentRoundType,

    Guid? CurrentQuestionId,
    int? CurrentQuestionOrder,

    DateTime? StartedOn,
    DateTime? LastActivityOn,
    DateTime? AssessmentExpiresOn,

    bool CanResume,
    bool IsInterrupted,

    IReadOnlyCollection<AssessmentWorkflowRoundResponse> Rounds);

public sealed record AssessmentWorkflowRoundResponse(
    Guid RoundId,
    int Order,
    string Type,
    string Status,

    int TotalQuestions,
    int CompletedQuestions,
    int SkippedQuestions,
    int RemainingQuestions,

    int CompletionPercentage,

    DateTime? StartedOn,
    DateTime? CompletedOn);