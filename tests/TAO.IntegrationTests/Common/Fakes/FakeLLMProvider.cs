using TAO.AI.Abstractions;
using TAO.AI.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.IntegrationTests.Common.Fakes;

public sealed class FakeLLMProvider : ILLMProvider
{
    public Task<Result<LLMResponse>> GenerateAsync(
        LLMRequest request,
        CancellationToken cancellationToken)
    {
        var response = new LLMResponse
        {
            Content = GetResponse(request.Prompt),
            ProviderName = "Fake",
            ModelName = "IntegrationTest"
        };

        return Task.FromResult(Result<LLMResponse>.Success(response));
    }

    private static string GetResponse(string prompt)
    {
        if (prompt.Contains("HIRING STRATEGY", StringComparison.OrdinalIgnoreCase))
        {
            return HiringStrategyResponse;
        }

        return JobProfileResponse;
    }

    private const string JobProfileResponse =
    """
    {
      "generatedMarkdown": "# Senior .NET Developer\n\n## Role Summary\nLooking for a Senior .NET Developer with 10+ years of experience.\n\n## Key Responsibilities\nNot specified.\n\n## Required Skills\n- .NET Core\n- Azure\n- SQL\n\n## Preferred Skills\n- Angular\n\n## Technologies\n- .NET Core\n- Azure\n- SQL\n- Angular\n\n## Minimum Experience\n10 years\n\n## Education\nNot specified.",
      "structuredProfile": {
        "roleTitle": "Senior .NET Developer",
        "roleSummary": "Looking for a Senior .NET Developer with 10+ years of experience.",
        "responsibilities": [],
        "requiredSkills": [
          ".NET Core",
          "Azure",
          "SQL"
        ],
        "preferredSkills": [
          "Angular"
        ],
        "technologies": [
          ".NET Core",
          "Azure",
          "SQL",
          "Angular"
        ],
        "minimumExperienceYears": 10,
        "education": []
      }
    }
    """;

    private const string HiringStrategyResponse =
    """
    {
      "generatedMarkdown": "# Hiring Strategy\n\n## Assessment Plan\n\n### Technical Assessment\n- Coding\n- System Design\n\n### AI Interview\nEvaluate architecture, communication and problem solving.",
      "structuredContent": {
        "assessmentStrategy": {
          "technicalAssessment": true,
          "codingAssessment": true,
          "systemDesign": true,
          "aiInterview": true
        },
        "competencies": [
          ".NET",
          "ASP.NET Core",
          "Azure",
          "SQL",
          "Angular"
        ],
        "screeningCriteria": [
          "10+ years experience",
          "Strong C#",
          "Cloud experience"
        ],
        "strongCandidateIndicators": [
          "Microservices",
          "Azure Architecture",
          "Leadership"
        ],
        "redFlags": [
          "No cloud experience",
          "Weak C# fundamentals"
        ],
        "aiInterviewGuidance": {
          "difficulty": "Senior",
          "durationMinutes": 90
        }
      }
    }
    """;
}