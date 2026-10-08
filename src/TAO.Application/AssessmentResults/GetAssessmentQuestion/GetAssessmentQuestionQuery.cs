using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentQuestion;

public sealed record GetAssessmentQuestionQuery(
    Guid AssessmentSessionId,
    Guid QuestionId)
    : IRequest<Result<GetAssessmentQuestionResponse>>;