using System.Text.Json;
using TAO.AI.Abstractions;
using TAO.AI.Contracts;
using TAO.AI.ResumeScreening.Contracts;
using TAO.AI.ResumeScreening.Parsers;
using TAO.AI.ResumeScreening.PromptTemplates;
using TAO.AI.ResumeScreening.Validators;
using TAO.SharedKernel.Results;

namespace TAO.AI.ResumeScreening;

internal sealed class ResumeScreeningGenerator
    : IResumeScreeningGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly ResumeScreeningPromptFactory _promptFactory;
    private readonly ResumeScreeningResponseParser _parser;
    private readonly ResumeScreeningResponseValidator _validator;

    public ResumeScreeningGenerator(
        ILLMProvider llmProvider,
        ResumeScreeningPromptFactory promptFactory,
        ResumeScreeningResponseParser parser,
        ResumeScreeningResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<Result<ResumeScreeningResult>> GenerateAsync(
        ResumeScreeningRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // ------------------------------------------------------------
        // Build Prompt
        // ------------------------------------------------------------

        var prompt = await _promptFactory.CreateAsync(
            request,
            cancellationToken);

        // ------------------------------------------------------------
        // Invoke LLM
        // ------------------------------------------------------------

        var llmResult = await _llmProvider.GenerateAsync(
            new LLMRequest
            {
                Prompt = prompt
            },
            cancellationToken);

        if (llmResult.IsFailure)
        {
            return Result<ResumeScreeningResult>.Failure(
                llmResult.Error!);
        }

        // ------------------------------------------------------------
        // Parse Response
        // ------------------------------------------------------------

        var parseResult = _parser.Parse(
            llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<ResumeScreeningResult>.Failure(
                parseResult.Error!);
        }

        // ------------------------------------------------------------
        // Validate Response
        // ------------------------------------------------------------

        var validationResult = _validator.Validate(
            parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<ResumeScreeningResult>.Failure(
                validationResult.Error!);
        }

        var aiResponse = parseResult.Value!;

        var generationResult = new ResumeScreeningResult
        {
            Prompt = prompt,
            RawResponse = llmResult.Value.Content,
            ProviderName = llmResult.Value.ProviderName,
            ModelName = llmResult.Value.ModelName,
            PromptVersion = 1,

            MarkdownContent = aiResponse.GeneratedMarkdown,

            StructuredContent = JsonSerializer.Serialize(
                aiResponse.StructuredContent),

            OverallMatchPercentage = aiResponse.OverallMatchPercentage,

            IsRecommended = aiResponse.IsRecommended
        };

        return Result<ResumeScreeningResult>.Success(
            generationResult);
    }
}