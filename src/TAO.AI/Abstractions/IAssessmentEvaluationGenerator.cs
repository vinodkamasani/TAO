using TAO.AI.AssessmentEvaluations.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IAssessmentEvaluationGenerator
{
    Task<Result<AssessmentEvaluationGenerationResult>> GenerateAsync(
        AssessmentSession session,
        IReadOnlyCollection<AssessmentRoundEvaluation> roundEvaluations,
        CancellationToken cancellationToken = default);
}