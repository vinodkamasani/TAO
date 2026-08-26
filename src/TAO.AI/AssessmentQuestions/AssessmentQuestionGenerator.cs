using TAO.AI.Abstractions;
using TAO.AI.AssessmentQuestions.Contracts;
using TAO.AI.AssessmentQuestions.Parsers;
using TAO.AI.AssessmentQuestions.PromptTemplates;
using TAO.AI.AssessmentQuestions.Validators;
using TAO.AI.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentQuestions;

internal sealed class AssessmentQuestionGenerator
    : IAssessmentQuestionGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly AssessmentQuestionPromptFactory _promptFactory;
    private readonly AssessmentQuestionResponseParser _parser;
    private readonly AssessmentQuestionResponseValidator _validator;

    public AssessmentQuestionGenerator(
        ILLMProvider llmProvider,
        AssessmentQuestionPromptFactory promptFactory,
        AssessmentQuestionResponseParser parser,
        AssessmentQuestionResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<Result<AssessmentQuestionGenerationResult>> GenerateAsync(
        JobProfile jobProfile,
        AssessmentSessionRound sessionRound,
         IReadOnlyCollection<string>? usedQuestionStarts = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jobProfile);
        ArgumentNullException.ThrowIfNull(sessionRound);

        // 1. Build prompt
        var prompt = await _promptFactory.CreateAsync(
            jobProfile,
            sessionRound,
            usedQuestionStarts,
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
            return Result<AssessmentQuestionGenerationResult>.Failure(
                llmResult.Error!);
        }

        // 3. Parse response
        var parseResult = _parser.Parse(
            llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<AssessmentQuestionGenerationResult>.Failure(
                parseResult.Error!);
        }

        // 4. Validate response
        var validationResult = _validator.Validate(
            parseResult.Value!,
            sessionRound);

        if (validationResult.IsFailure)
        {
            return Result<AssessmentQuestionGenerationResult>.Failure(
                validationResult.Error!);
        }

        var aiResponse = parseResult.Value!;

        var generationResult =
            new AssessmentQuestionGenerationResult
            {
                Prompt = prompt,
                RawResponse = llmResult.Value.Content,
                ProviderName = llmResult.Value.ProviderName,
                ModelName = llmResult.Value.ModelName,
                PromptVersion = 1,
                Response = aiResponse
            };

        return Result<AssessmentQuestionGenerationResult>.Success(
            generationResult);
    }
}