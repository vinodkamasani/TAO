using MediatR;
using TAO.Application.AssessmentSessions.Advance;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Skip;

public sealed record SkipAssessmentQuestionCommand(
    Guid AssessmentQuestionId)
    : IRequest<Result<AdvanceAssessmentSessionResponse>>;