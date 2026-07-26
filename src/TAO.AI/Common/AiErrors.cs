using TAO.SharedKernel;

namespace TAO.AI.Common;

public static class AiErrors
{
    public static readonly Error EmptyResponse =
        Error.Validation(
            "AI.EmptyResponse",
            "The AI provider returned an empty response.");

    public static readonly Error InvalidJsonResponse =
        Error.Validation(
            "AI.InvalidJsonResponse",
            "The AI response is not valid JSON.");

    public static readonly Error InvalidResponse =
        Error.Validation(
            "AI.InvalidResponse",
            "The AI response could not be parsed.");

    public static readonly Error GeneratedMarkdownMissing =
    Error.Validation(
        "AI.GeneratedMarkdownMissing",
        "The AI response does not contain generated markdown.");

    public static readonly Error InvalidStructuredProfile =
        Error.Validation(
            "AI.InvalidStructuredProfile",
            "The structured profile is invalid.");

    public static readonly Error RoleTitleMissing =
        Error.Validation(
            "AI.RoleTitleMissing",
            "The structured profile does not contain a role title.");

    public static readonly Error RequiredSkillsMissing =
        Error.Validation(
            "AI.RequiredSkillsMissing",
            "The structured profile does not contain required skills.");

    public static readonly Error ProviderRequestFailed =
    Error.Failure(
        "AI.ProviderRequestFailed",
        "The AI provider returned an unsuccessful response.");

    public static readonly Error ProviderUnavailable =
        Error.Failure(
            "AI.ProviderUnavailable",
            "The AI provider is unavailable.");

    public static readonly Error ProviderTimeout =
        Error.Failure(
            "AI.ProviderTimeout",
            "The AI provider request timed out.");

    public static readonly Error InvalidProviderResponse =
        Error.Failure(
            "AI.InvalidProviderResponse",
            "The AI provider returned an invalid response.");
}