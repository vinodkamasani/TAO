using TAO.AI.Abstractions;
using TAO.AI.AssessmentRoundEvaluations.Contracts;
using TAO.AI.AssessmentRoundEvaluations.Parsers;
using TAO.AI.AssessmentRoundEvaluations.PromptTemplates;
using TAO.AI.AssessmentRoundEvaluations.Validators;
using TAO.AI.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentRoundEvaluations;

internal sealed class AssessmentRoundEvaluationGenerator
    : IAssessmentRoundEvaluationGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly AssessmentRoundEvaluationPromptFactory _promptFactory;
    private readonly AssessmentRoundEvaluationResponseParser _parser;
    private readonly AssessmentRoundEvaluationResponseValidator _validator;

    public AssessmentRoundEvaluationGenerator(
        ILLMProvider llmProvider,
        AssessmentRoundEvaluationPromptFactory promptFactory,
        AssessmentRoundEvaluationResponseParser parser,
        AssessmentRoundEvaluationResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<
        Result<AssessmentRoundEvaluationGenerationResult>>
        GenerateAsync(
            AssessmentSessionRound sessionRound,
            IReadOnlyCollection<AssessmentQuestionEvaluation> evaluations,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sessionRound);
        ArgumentNullException.ThrowIfNull(evaluations);

        var prompt = await _promptFactory.CreateAsync(
            sessionRound,
            evaluations,
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
                AssessmentRoundEvaluationGenerationResult>
                .Failure(llmResult.Error!);
        }

        var parseResult = _parser.Parse(
            llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<
                AssessmentRoundEvaluationGenerationResult>
                .Failure(parseResult.Error!);
        }

        var validationResult = _validator.Validate(
            parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<
                AssessmentRoundEvaluationGenerationResult>
                .Failure(validationResult.Error!);
        }

        var response = parseResult.Value!;

        return Result<
            AssessmentRoundEvaluationGenerationResult>
            .Success(
                new AssessmentRoundEvaluationGenerationResult
                {
                    Prompt = prompt,
                    RawResponse = llmResult.Value.Content,
                    ProviderName = llmResult.Value.ProviderName,
                    ModelName = llmResult.Value.ModelName,
                    PromptVersion = 1,
                    Confidence = response.Confidence,
                    Strengths = response.Strengths,
                    Gaps = response.Gaps,
                    Evidence = response.Evidence
                });
    }
}