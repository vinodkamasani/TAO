namespace TAO.AI.ResumeParsing.Contracts;

public sealed class ResumeParsingResult
{
    public required string Prompt { get; init; }

    public required string RawResponse { get; init; }

    public required string ProviderName { get; init; }

    public required string ModelName { get; init; }

    public required int PromptVersion { get; init; }

    public required string StructuredContent { get; init; }
    public required string ExtractedText { get; init; }
    public required string CandidateName { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public string? LinkedInUrl { get; init; }
    public string? CurrentCompany { get; init; }
    public string? CurrentLocation { get; init; }
}