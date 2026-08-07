using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.ResumeScreenings.Create;

public sealed record CreateResumeScreeningCommand(
    Guid CandidateApplicationId)
    : IRequest<Result<Guid>>;