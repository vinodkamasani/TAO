using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.FollowUp;

public sealed record GenerateFollowUpCommand(
    Guid AssessmentQuestionId)
    : IRequest<Result<GenerateFollowUpResponse>>;