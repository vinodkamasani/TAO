using TAO.AI.AssessmentQuestions.Contracts;
using TAO.Domain.Entities;
using TAO.Domain.Enums;
using TAO.SharedKernel;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentQuestions.Validators;

internal sealed class AssessmentQuestionResponseValidator
{
    private static readonly HashSet<string> DsaCompetencies =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Arrays",
            "Hashing",
            "Two Pointers",
            "Sliding Window",
            "Linked Lists",
            "Stacks",
            "Queues",
            "Binary Search",
            "Trees",
            "Graphs",
            "Heaps",
            "Backtracking",
            "Dynamic Programming",
            "Greedy Algorithms",
            "Recursion",
            "Topological Sort",
            "Strings",
            "Sorting"
        };

    public Result Validate(
        AssessmentQuestionAiResponse response,
        AssessmentSessionRound sessionRound)
    {
        if (response is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.InvalidResponse",
                    "The AI response is empty."));
        }

        if (string.IsNullOrWhiteSpace(response.Question))
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.QuestionRequired",
                    "The AI response must contain a question."));
        }

        if (response.Competencies is null ||
            response.Competencies.Count == 0)
        {
            return Result.Failure(
                Error.Validation(
                    "AssessmentQuestion.CompetenciesRequired",
                    "The AI response must contain at least one competency."));
        }

        foreach (var competency in response.Competencies)
        {
            if (string.IsNullOrWhiteSpace(competency))
            {
                return Result.Failure(
                    Error.Validation(
                        "AssessmentQuestion.InvalidCompetency",
                        "The AI response contains an empty competency."));
            }

            var isValid =
                sessionRound.Type == AssessmentRoundType.Dsa
                    ? DsaCompetencies.Contains(competency.Trim())
                    : IsConfiguredCompetency(
                        competency,
                        sessionRound);

            if (!isValid)
            {
                return Result.Failure(
                    Error.Validation(
                        "AssessmentQuestion.InvalidCompetency",
                        sessionRound.Type == AssessmentRoundType.Dsa
                            ? $"The AI selected competency '{competency}', " +
                              "which is not a valid DSA competency."
                            : $"The AI selected competency '{competency}', " +
                              "which is not configured for the assessment round."));
            }
        }

        return Result.Success();
    }

    private static bool IsConfiguredCompetency(
        string competency,
        AssessmentSessionRound sessionRound)
    {
        var allowedCompetencies =
            sessionRound.Competencies
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return allowedCompetencies.Contains(
            competency.Trim());
    }
}