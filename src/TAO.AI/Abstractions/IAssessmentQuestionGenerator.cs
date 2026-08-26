using TAO.AI.AssessmentQuestions.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IAssessmentQuestionGenerator
{
    Task<Result<AssessmentQuestionGenerationResult>> GenerateAsync(
        JobProfile jobProfile,
        AssessmentSessionRound sessionRound,
          IReadOnlyCollection<string>? usedQuestionStarts = null,
        CancellationToken cancellationToken = default);
}