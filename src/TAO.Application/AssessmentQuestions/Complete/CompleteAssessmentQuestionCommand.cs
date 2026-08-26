using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Complete;

public sealed record CompleteAssessmentQuestionCommand(
    Guid AssessmentQuestionId)
    : IRequest<Result>;