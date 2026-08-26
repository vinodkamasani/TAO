using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestionEvaluations.Evaluate;

public sealed record EvaluateAssessmentQuestionCommand(
    Guid AssessmentQuestionId)
    : IRequest<Result>;