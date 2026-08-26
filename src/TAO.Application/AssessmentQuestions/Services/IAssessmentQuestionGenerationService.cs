using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Services;

public interface IAssessmentQuestionGenerationService
{
    Task<Result<AssessmentQuestion>> GenerateNextAsync(
        AssessmentSession session,
        AssessmentSessionRound sessionRound,
        CancellationToken cancellationToken);
}