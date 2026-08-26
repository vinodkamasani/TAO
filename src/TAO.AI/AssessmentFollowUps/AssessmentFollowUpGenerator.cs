using TAO.AI.Abstractions;
using TAO.AI.AssessmentFollowUps.Contracts;
using TAO.AI.AssessmentFollowUps.Parsers;
using TAO.AI.AssessmentFollowUps.PromptTemplates;
using TAO.AI.AssessmentFollowUps.Validators;
using TAO.AI.Contracts;
using TAO.Domain.Entities;
using TAO.SharedKernel.Results;

namespace TAO.AI.AssessmentFollowUps;

internal sealed class AssessmentFollowUpGenerator
    : IAssessmentFollowUpGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly AssessmentFollowUpPromptFactory _promptFactory;
    private readonly AssessmentFollowUpResponseParser _parser;
    private readonly AssessmentFollowUpResponseValidator _validator;

    public AssessmentFollowUpGenerator(
        ILLMProvider llmProvider,
        AssessmentFollowUpPromptFactory promptFactory,
        AssessmentFollowUpResponseParser parser,
        AssessmentFollowUpResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<Result<AssessmentFollowUpGenerationResult>> GenerateAsync(
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
            return Result<AssessmentFollowUpGenerationResult>.Failure(
                llmResult.Error!);
        }

        var parseResult = _parser.Parse(
            llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<AssessmentFollowUpGenerationResult>.Failure(
                parseResult.Error!);
        }

        var validationResult = _validator.Validate(
            parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<AssessmentFollowUpGenerationResult>.Failure(
                validationResult.Error!);
        }

        return Result<AssessmentFollowUpGenerationResult>.Success(
            new AssessmentFollowUpGenerationResult
            {
                Prompt = prompt,
                RawResponse = llmResult.Value.Content,
                ProviderName = llmResult.Value.ProviderName,
                ModelName = llmResult.Value.ModelName,
                PromptVersion = 1,
                Response = parseResult.Value!
            });
    }
}