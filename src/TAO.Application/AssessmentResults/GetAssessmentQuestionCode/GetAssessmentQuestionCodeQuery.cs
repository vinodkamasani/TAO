using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentQuestionCode;

public sealed record GetAssessmentQuestionCodeQuery(
    Guid AssessmentSessionId,
    Guid QuestionId)
    : IRequest<Result<GetAssessmentQuestionCodeResponse>>;