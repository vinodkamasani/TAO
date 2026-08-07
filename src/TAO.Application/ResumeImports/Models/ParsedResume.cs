

namespace TAO.Application.ResumeImports.Models;

public sealed record ParsedResume(
    string FullName,
    string Email,
    string? Phone,
    string StructuredContent,
    string RawResponse);