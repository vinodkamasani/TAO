using TAO.AI.AssessmentFollowUps.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.Abstractions;

public interface IAssessmentFollowUpGenerator
{
    Task<Result<AssessmentFollowUpGenerationResult>> GenerateAsync(
        AssessmentQuestion question,
        AssessmentSessionRound sessionRound,
        CancellationToken cancellationToken = default);
}