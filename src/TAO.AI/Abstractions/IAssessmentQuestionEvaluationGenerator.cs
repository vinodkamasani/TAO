using TAO.Domain.Entities;
using TAO.SharedKernel.Results;
using TAO.AI.AssessmentQuestionEvaluations.Contracts;

namespace TAO.AI.AssessmentQuestionEvaluations;

public interface IAssessmentQuestionEvaluationGenerator
{
    Task<Result<AssessmentQuestionEvaluationGenerationResult>> GenerateAsync(
        AssessmentQuestion question,
        AssessmentSessionRound sessionRound,
        CancellationToken cancellationToken = default);
}