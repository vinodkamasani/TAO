using MediatR;
using TAO.SharedKernel.Results;

namespace TAO.Application.Candidates.Signup;

public sealed record CandidateSignupCommand(
    Guid InvitationId,
    string Email,
    string Password)
    : IRequest<Result>;