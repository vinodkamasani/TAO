using System.Text.Json;
using TAO.AI.Abstractions;
using TAO.AI.Contracts;
using TAO.AI.AssessmentStrategies.Contracts;
using TAO.AI.AssessmentStrategies.Parsers;
using TAO.AI.AssessmentStrategies.PromptTemplates;
using TAO.AI.AssessmentStrategies.Validators;
using TAO.Domain.Entities;
using TAO.Domain.ValueObjects;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentStrategies;

internal sealed class AssessmentStrategyGenerator
    : IAssessmentStrategyGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly AssessmentStrategyPromptFactory _promptFactory;
    private readonly AssessmentStrategyResponseParser _parser;
    private readonly AssessmentStrategyResponseValidator _validator;

    public AssessmentStrategyGenerator(
        ILLMProvider llmProvider,
        AssessmentStrategyPromptFactory promptFactory,
        AssessmentStrategyResponseParser parser,
        AssessmentStrategyResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<Result<AssessmentStrategyGenerationResult>> GenerateAsync(
        JobProfile jobProfile,
        HiringStrategy hiringStrategy,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jobProfile);
        ArgumentNullException.ThrowIfNull(hiringStrategy);

        // 1. Build prompt
        var prompt = await _promptFactory.CreateAsync(
            jobProfile.StructuredProfile.Value,
            hiringStrategy.StructuredContent.Value,
            cancellationToken);

        // 2. Call LLM
        var llmResult = await _llmProvider.GenerateAsync(
            new LLMRequest
            {
                Prompt = prompt
            },
            cancellationToken);

        if (llmResult.IsFailure)
        {
            return Result<AssessmentStrategyGenerationResult>.Failure(
                llmResult.Error!);
        }

        // 3. Parse response
        var parseResult = _parser.Parse(
            llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<AssessmentStrategyGenerationResult>.Failure(
                parseResult.Error!);
        }

        // 4. Validate response
        var validationResult = _validator.Validate(
            parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<AssessmentStrategyGenerationResult>.Failure(
                validationResult.Error!);
        }

        var aiResponse = parseResult.Value!;

        // 5. Create generation result
        var structuredJson = JsonSerializer.Serialize(
            aiResponse);

        var generationResult =
            new AssessmentStrategyGenerationResult
            {
                Prompt = prompt,
                RawResponse = llmResult.Value.Content,
                ProviderName = llmResult.Value.ProviderName,
                ModelName = llmResult.Value.ModelName,
                PromptVersion = 1,
                StructuredContent = new StructuredContent(
                    structuredJson)
            };

        return Result<AssessmentStrategyGenerationResult>.Success(
            generationResult);
    }
}