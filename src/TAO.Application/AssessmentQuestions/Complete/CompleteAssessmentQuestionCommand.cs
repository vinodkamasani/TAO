using MediatR;
using TAO.Application.AssessmentSessions.Advance;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Complete;

public sealed record CompleteAssessmentQuestionCommand(
    Guid AssessmentQuestionId)
    : IRequest<Result<AdvanceAssessmentSessionResponse>>;