using TAO.AI.AssessmentRoundEvaluations.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IAssessmentRoundEvaluationGenerator
{
    Task<Result<AssessmentRoundEvaluationGenerationResult>> GenerateAsync(
        AssessmentSessionRound sessionRound,
        IReadOnlyCollection<AssessmentQuestionEvaluation> evaluations,
        CancellationToken cancellationToken = default);
}
