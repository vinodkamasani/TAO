namespace TAO.Api.Endpoints.Candidates.Signup;

public sealed record CandidateSignupRequest(
    string Email,
    string Password);