using TAO.AI.Abstractions;
using TAO.AI.AssessmentStrategies.Contracts;
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
        if (prompt.Contains("HIRING STRATEGY", StringComparison.OrdinalIgnoreCase) && !prompt.Contains("ASSESSMENT STRATEGY", StringComparison.OrdinalIgnoreCase))
        {
            return HiringStrategyResponse;
        }

        if (prompt.Contains(
      "ASSESSMENTNAME",
      StringComparison.OrdinalIgnoreCase))
        {
            return AssessmentStrategyResponse;
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

    private const string AssessmentStrategyResponse =
"""
{
  "assessmentName": "Senior .NET Developer Assessment",
  "rounds": [
    {
      "order": 1,
      "type": "Dsa",
      "difficulty": "Medium",
      "durationInMinutes": 30,
      "questionCount": 2,
      "competencies": [
        {
          "name": "C#",
          "priority": "High"
        },
        {
          "name": "Object-Oriented Programming",
          "priority": "High"
        }
      ]
    },
    {
      "order": 2,
      "type": "Coding",
      "difficulty": "Medium",
      "durationInMinutes": 60,
      "questionCount": 3,
      "competencies": [
        {
          "name": "C#",
          "priority": "High"
        },
        {
          "name": "ASP.NET Core",
          "priority": "High"
        },
        {
          "name": "Entity Framework Core",
          "priority": "High"
        }
      ]
    },
    {
      "order": 3,
      "type": "TechnicalDiscussion",
      "difficulty": "Medium",
      "durationInMinutes": 45,
      "questionCount": 2,
      "competencies": [
        {
          "name": "Object-Oriented Programming",
          "priority": "High"
        },
        {
          "name": "SOLID Principles",
          "priority": "High"
        },
        {
          "name": "Angular",
          "priority": "High"
        }
      ]
    },
    {
      "order": 4,
      "type": "SystemDesign",
      "difficulty": "Hard",
      "durationInMinutes": 75,
      "questionCount": 1,
      "competencies": [
        {
          "name": "REST API Development",
          "priority": "High"
        },
        {
          "name": "SQL Server",
          "priority": "High"
        },
        {
          "name": "Microservices Architecture",
          "priority": "Low"
        },
        {
          "name": "Microsoft Azure",
          "priority": "Low"
        }
      ]
    }
  ]
}
""";
}