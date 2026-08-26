using TAO.Domain.ValueObjects;

namespace TAO.AI.AssessmentQuestions.Contracts;

public sealed record AssessmentQuestionGenerationResult
{
    public required string Prompt { get; init; }

    public required string RawResponse { get; init; }

    public required string ProviderName { get; init; }

    public required string ModelName { get; init; }

    public required int PromptVersion { get; init; }

    public required AssessmentQuestionAiResponse Response { get; init; }
}