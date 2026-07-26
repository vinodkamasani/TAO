using System.Text.Json;
using TAO.AI.Abstractions;
using TAO.AI.Contracts;
using TAO.AI.JobProfiles.Contracts;
using TAO.AI.JobProfiles.Parsers;
using TAO.AI.JobProfiles.PromptTemplates;
using TAO.AI.JobProfiles.Validators;
using TAO.SharedKernel.Results;

namespace TAO.AI.JobProfiles;

internal sealed class JobProfileGenerator : IJobProfileGenerator
{
    private readonly ILLMProvider _llmProvider;
    private readonly JobProfilePromptFactory _promptFactory;
    private readonly JobProfileResponseParser _parser;
    private readonly JobProfileResponseValidator _validator;

    public JobProfileGenerator(
        ILLMProvider llmProvider,
        JobProfilePromptFactory promptFactory,
        JobProfileResponseParser parser,
        JobProfileResponseValidator validator)
    {
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<Result<JobProfileGenerationResult>> GenerateAsync(
       string jobDescription,
       CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jobDescription);

        // 1. Build prompt
        var prompt = await _promptFactory.CreateAsync(
            jobDescription,
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
            return Result<JobProfileGenerationResult>.Failure(
                llmResult.Error!);
        }

        // 3. Parse response
        var parseResult = _parser.Parse(llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<JobProfileGenerationResult>.Failure(
                parseResult.Error!);
        }

        // 4. Validate response
        var validationResult = _validator.Validate(parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<JobProfileGenerationResult>.Failure(
                validationResult.Error!);
        }

        var aiResponse = parseResult.Value!;

        var generationResult = new JobProfileGenerationResult
        {
            Prompt = prompt,
            RawResponse = llmResult.Value!.Content,
            ProviderName = llmResult.Value.ProviderName,
            ModelName = llmResult.Value.ModelName,
            PromptVersion = 1,

            GeneratedContent = aiResponse.GeneratedMarkdown,

            StructuredProfile = JsonSerializer.Serialize(
                aiResponse.StructuredProfile)
        };

        return Result<JobProfileGenerationResult>.Success(generationResult);
    }
}