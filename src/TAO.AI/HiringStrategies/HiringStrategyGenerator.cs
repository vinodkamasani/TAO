using System.Text.Json;
using TAO.AI.Abstractions;
using TAO.AI.Contracts;
using TAO.AI.HiringStrategies.Contracts;
using TAO.AI.HiringStrategies.Parsers;
using TAO.AI.HiringStrategies.PromptTemplates;
using TAO.AI.HiringStrategies.Validators;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.HiringStrategies;

internal sealed class HiringStrategyGenerator : IHiringStrategyGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly HiringStrategyPromptFactory _promptFactory;
    private readonly HiringStrategyResponseParser _parser;
    private readonly HiringStrategyResponseValidator _validator;

    public HiringStrategyGenerator(
        ILLMProvider llmProvider,
        HiringStrategyPromptFactory promptFactory,
        HiringStrategyResponseParser parser,
        HiringStrategyResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<Result<HiringStrategyGenerationResult>> GenerateAsync(
        JobProfile jobProfile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jobProfile);

        // 1. Build prompt
        var prompt = await _promptFactory.CreateAsync(
            jobProfile,
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
            return Result<HiringStrategyGenerationResult>.Failure(
                llmResult.Error!);
        }

        // 3. Parse response
        var parseResult = _parser.Parse(llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<HiringStrategyGenerationResult>.Failure(
                parseResult.Error!);
        }

        // 4. Validate response
        var validationResult = _validator.Validate(parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<HiringStrategyGenerationResult>.Failure(
                validationResult.Error!);
        }

        var aiResponse = parseResult.Value!;

        var generationResult = new HiringStrategyGenerationResult
        {
            Prompt = prompt,
            RawResponse = llmResult.Value.Content,
            ProviderName = llmResult.Value.ProviderName,
            ModelName = llmResult.Value.ModelName,
            PromptVersion = 1,

            Content = aiResponse.GeneratedMarkdown,

            StructuredContent = JsonSerializer.Serialize(
                aiResponse.StructuredContent)
        };

        return Result<HiringStrategyGenerationResult>.Success(
            generationResult);
    }
}