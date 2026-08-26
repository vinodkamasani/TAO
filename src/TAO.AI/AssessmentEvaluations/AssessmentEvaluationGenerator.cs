using TAO.AI.Abstractions;
using TAO.AI.AssessmentEvaluations.Contracts;
using TAO.AI.AssessmentEvaluations.Parsers;
using TAO.AI.AssessmentEvaluations.PromptTemplates;
using TAO.AI.AssessmentEvaluations.Validators;
using TAO.AI.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentEvaluations;

internal sealed class AssessmentEvaluationGenerator
    : IAssessmentEvaluationGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly AssessmentEvaluationPromptFactory _promptFactory;
    private readonly AssessmentEvaluationResponseParser _parser;
    private readonly AssessmentEvaluationResponseValidator _validator;

    public AssessmentEvaluationGenerator(
        ILLMProvider llmProvider,
        AssessmentEvaluationPromptFactory promptFactory,
        AssessmentEvaluationResponseParser parser,
        AssessmentEvaluationResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<
        Result<AssessmentEvaluationGenerationResult>>
        GenerateAsync(
            AssessmentSession session,
            IReadOnlyCollection<AssessmentRoundEvaluation> roundEvaluations,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(roundEvaluations);

        var prompt = await _promptFactory.CreateAsync(
            session,
            roundEvaluations,
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
                AssessmentEvaluationGenerationResult>
                .Failure(llmResult.Error!);
        }

        var parseResult = _parser.Parse(
            llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<
                AssessmentEvaluationGenerationResult>
                .Failure(parseResult.Error!);
        }

        var validationResult = _validator.Validate(
            parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<
                AssessmentEvaluationGenerationResult>
                .Failure(validationResult.Error!);
        }

        var response = parseResult.Value!;

        return Result<
            AssessmentEvaluationGenerationResult>
            .Success(
                new AssessmentEvaluationGenerationResult
                {
                    Prompt = prompt,
                    RawResponse = llmResult.Value.Content,
                    ProviderName = llmResult.Value.ProviderName,
                    ModelName = llmResult.Value.ModelName,
                    PromptVersion = 1,
                    Confidence = response.Confidence,
                    ExecutiveSummary =
                        response.ExecutiveSummary,
                    Strengths = response.Strengths,
                    Gaps = response.Gaps,
                    Evidence = response.Evidence
                });
    }
}