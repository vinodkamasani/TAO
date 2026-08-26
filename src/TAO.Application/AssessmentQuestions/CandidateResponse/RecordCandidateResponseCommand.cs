using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.AssessmentQuestions.CandidateResponse;

public sealed record RecordCandidateResponseCommand(
  Guid AssessmentQuestionId,
  string Response)
  : IRequest<Result>;
