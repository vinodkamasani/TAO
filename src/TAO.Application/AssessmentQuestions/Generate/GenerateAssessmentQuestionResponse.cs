namespace TAO.Application.AssessmentQuestions.Generate;

public sealed record GenerateAssessmentQuestionResponse(
    Guid QuestionId,
    int Order,
    string Question,
    IReadOnlyCollection<string> Competencies);