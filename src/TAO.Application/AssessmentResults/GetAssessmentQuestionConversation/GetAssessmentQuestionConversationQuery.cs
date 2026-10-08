using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentResults.GetAssessmentQuestionConversation;

public sealed record GetAssessmentQuestionConversationQuery(
    Guid AssessmentSessionId,
    Guid QuestionId)
    : IRequest<Result<GetAssessmentQuestionConversationResponse>>;