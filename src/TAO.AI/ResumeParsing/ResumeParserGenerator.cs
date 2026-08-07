using System.Text.Json;
using TAO.AI.Abstractions;
using TAO.AI.Contracts;
using TAO.AI.ResumeParsing.Contracts;
using TAO.AI.ResumeParsing.Parsers;
using TAO.AI.ResumeParsing.PromptTemplates;
using TAO.AI.ResumeParsing.Validators;
using TAO.SharedKernel.AI;
using TAO.SharedKernel.AI.Models;
using TAO.SharedKernel.Results;

namespace TAO.AI.ResumeParsing;

internal sealed class ResumeParserGenerator : IResumeParserGenerator
{
    private readonly IDocumentTextExtractor _documentTextExtractor;
    private readonly ILLMProvider _llmProvider;
    private readonly ResumePromptFactory _promptFactory;
    private readonly ResumeResponseParser _parser;
    private readonly ResumeResponseValidator _validator;

    public ResumeParserGenerator(
        IDocumentTextExtractor documentTextExtractor,
        ILLMProvider llmProvider,
        ResumePromptFactory promptFactory,
        ResumeResponseParser parser,
        ResumeResponseValidator validator)
    {
        _documentTextExtractor = documentTextExtractor;
        _llmProvider = llmProvider;
        _promptFactory = promptFactory;
        _parser = parser;
        _validator = validator;
    }

    public async Task<Result<ResumeParsingResult>> ParseAsync(
        UploadedResume uploadedResume,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uploadedResume);

        // ------------------------------------------------------------------
        // Extract text from the uploaded document
        // ------------------------------------------------------------------

        var extractionResult =
            await _documentTextExtractor.ExtractTextAsync(
                uploadedResume,
                cancellationToken);

        if (extractionResult.IsFailure)
        {
            return Result<ResumeParsingResult>.Failure(
                extractionResult.Error!);
        }

        // ------------------------------------------------------------------
        // Build prompt
        // ------------------------------------------------------------------

        var prompt = await _promptFactory.CreateAsync(
            extractionResult.Value!,
            cancellationToken);

        // ------------------------------------------------------------------
        // Invoke LLM
        // ------------------------------------------------------------------

        var llmResult = await _llmProvider.GenerateAsync(
            new LLMRequest
            {
                Prompt = prompt
            },
            cancellationToken);

        if (llmResult.IsFailure)
        {
            return Result<ResumeParsingResult>.Failure(
                llmResult.Error!);
        }

        // ------------------------------------------------------------------
        // Parse response
        // ------------------------------------------------------------------

        var parseResult =
            _parser.Parse(llmResult.Value!.Content);

        if (parseResult.IsFailure)
        {
            return Result<ResumeParsingResult>.Failure(
                parseResult.Error!);
        }

        // ------------------------------------------------------------------
        // Validate response
        // ------------------------------------------------------------------

        var validationResult =
            _validator.Validate(parseResult.Value!);

        if (validationResult.IsFailure)
        {
            return Result<ResumeParsingResult>.Failure(
                validationResult.Error!);
        }

        var aiResponse = parseResult.Value!;

        // Helper to safely extract string properties from the structured JSON
        static string ExtractString(JsonElement element, string propertyName)
        {
            if (element.ValueKind != JsonValueKind.Object)
                return string.Empty;

            return element.TryGetProperty(propertyName, out var prop) &&
                   prop.ValueKind == JsonValueKind.String
                ? prop.GetString()!
                : string.Empty;
        }

        var extractedText = extractionResult.Value!;

        var candidateName = ExtractString(aiResponse.StructuredContent, "fullName");
        var email = ExtractString(aiResponse.StructuredContent, "email");
        var phone = ExtractString(aiResponse.StructuredContent, "phoneNumber");
        var linkedIn = ExtractString(aiResponse.StructuredContent, "linkedInUrl");
        var currentCompany = ExtractString(aiResponse.StructuredContent, "currentCompany");
        var currentLocation = ExtractString(aiResponse.StructuredContent, "location");

        var generationResult = new ResumeParsingResult
        {
            Prompt = prompt,
            RawResponse = llmResult.Value.Content,
            ProviderName = llmResult.Value.ProviderName,
            ModelName = llmResult.Value.ModelName,
            PromptVersion = 1,

            StructuredContent = JsonSerializer.Serialize(aiResponse.StructuredContent),

            // Required fields
            ExtractedText = extractedText,
            CandidateName = string.IsNullOrWhiteSpace(candidateName) ? string.Empty : candidateName,
            Email = string.IsNullOrWhiteSpace(email) ? string.Empty : email,

            // Optional fields
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
            LinkedInUrl = string.IsNullOrWhiteSpace(linkedIn) ? null : linkedIn,
            CurrentCompany = string.IsNullOrWhiteSpace(currentCompany) ? null : currentCompany,
            CurrentLocation = string.IsNullOrWhiteSpace(currentLocation) ? null : currentLocation
        };

        return Result<ResumeParsingResult>.Success(
            generationResult);
    }
}