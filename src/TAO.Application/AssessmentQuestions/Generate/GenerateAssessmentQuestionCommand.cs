using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.Generate;

public sealed record GenerateAssessmentQuestionCommand(
    Guid AssessmentSessionId)
    : IRequest<Result<GenerateAssessmentQuestionResponse>>;