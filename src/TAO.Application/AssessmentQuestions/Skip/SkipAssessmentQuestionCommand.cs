using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Skip;

public sealed record SkipAssessmentQuestionCommand(
    Guid AssessmentQuestionId)
    : IRequest<Result>;