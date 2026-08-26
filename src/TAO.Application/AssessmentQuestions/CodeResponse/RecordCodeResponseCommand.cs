using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.CodeResponse;

public sealed record RecordCodeResponseCommand(
    Guid AssessmentQuestionId,
    string Code)
    : IRequest<Result>;