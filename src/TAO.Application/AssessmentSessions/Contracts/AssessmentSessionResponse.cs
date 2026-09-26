using TAO.Domain.ValueObjects;

namespace TAO.Application.AssessmentSessions.Contracts;

public sealed record AssessmentSessionResponse(
    Guid Id,
    Guid CandidateApplicationId,
    Guid AssessmentStrategyId,
    string Status,
    AssessmentStrategySnapshot StrategySnapshot,
    Guid? CurrentSessionRoundId,
    Guid? CurrentQuestionId,
    DateTime? ConsentAcceptedOn,
    int ConsentVersion,
    DateTime? StartedOn,
    DateTime? CompletedOn,
    DateTime AssessmentExpiresOn,
    DateTime LastActivityOn,
    bool HasUsedInterruptionWindow,
    bool IsInterrupted,
    IReadOnlyCollection<AssessmentSessionRoundResponse> Rounds);

public sealed record AssessmentSessionRoundResponse(
    Guid Id,
    Guid AssessmentRoundId,
    int Order,
    string Type,
    string Difficulty,
    int DurationInMinutes,
    int TargetQuestionCount,
    string Status,
    DateTime? StartedOn,
    DateTime? ExpiresOn,
    DateTime? CompletedOn,
    IReadOnlyCollection<AssessmentRoundCompetencyResponse> Competencies,
    IReadOnlyCollection<AssessmentQuestionResponse> Questions);

public sealed record AssessmentRoundCompetencyResponse(
    string Name,
    string Priority,
    int MinimumPassPercentage);

public sealed record AssessmentQuestionResponse(
    Guid Id,
    int Order,
    string PrimaryQuestion,
    string Status,
    IReadOnlyCollection<string> Competencies,
    ConversationContent? Conversation,
    string? CandidateCode,
    DateTime? StartedOn,
    DateTime? CompletedOn);