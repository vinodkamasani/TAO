using TAO.AI.Abstractions;
using TAO.AI.AssessmentQuestionEvaluations.Contracts;
using TAO.AI.AssessmentQuestionEvaluations.Parsers;
using TAO.AI.AssessmentQuestionEvaluations.PromptTemplates;
using TAO.AI.AssessmentQuestionEvaluations.Validators;
using TAO.AI.Contracts;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentQuestionEvaluations;

internal sealed class AssessmentQuestionEvaluationGenerator
    : IAssessmentQuestionEvaluationGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly AssessmentQuestionEvaluationPromptFactory _promptFactory;
    private readonly AssessmentQuestionEvaluationResponseParser _parser;
    private readonly AssessmentQuestionEvaluationResponseValidator _validator;

    public AssessmentQuestionEvaluationGenerator(
        ILLMProvider llmProvider,
        AssessmentQuestionEvaluationPromptFactory promptFactory,
        AssessmentQuestionEvaluationResponseParser parser,
        AssessmentQuestionEvaluationResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<
        Result<AssessmentQuestionEvaluationGenerationResult>>
        GenerateAsync(
            AssessmentQuestion question,
            AssessmentSessionRound sessionRound,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(sessionRound);

        var prompt = await _promptFactory.CreateAsync(
            question,
            sessionRound,
            cancellationToken);

        var llmResult = await _llmProvider.GenerateAsync(
            new LLMRequest
            {
                Prompt = prompt
            },
            cancellationToken);

        if (llmResult.IsFailure)
        {
            return Result<
                AssessmentQuestionEvaluationGenerationResult>
                .Failure(llmResult.Error!);
        }

        var parseResult = _parser.Parse(
            llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<
                AssessmentQuestionEvaluationGenerationResult>
                .Failure(parseResult.Error!);
        }

        var validationResult = _validator.Validate(
            parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<
                AssessmentQuestionEvaluationGenerationResult>
                .Failure(validationResult.Error!);
        }

        var response = parseResult.Value!;

        var competencies =
            response.Competencies
                .Select(
                    x => AssessmentQuestionCompetencyEvaluation.Create(
                        x.Name,
                        x.Score))
                .ToList();

        return Result<
            AssessmentQuestionEvaluationGenerationResult>
            .Success(
                new AssessmentQuestionEvaluationGenerationResult
                {
                    Prompt = prompt,
                    RawResponse = llmResult.Value.Content,
                    ProviderName = llmResult.Value.ProviderName,
                    ModelName = llmResult.Value.ModelName,
                    PromptVersion = 1,
                    Score = response.Score,
                    Confidence = response.Confidence,
                    Strengths = response.Strengths,
                    Gaps = response.Gaps,
                    Evidence = response.Evidence,
                    Competencies = competencies
                });
    }
}