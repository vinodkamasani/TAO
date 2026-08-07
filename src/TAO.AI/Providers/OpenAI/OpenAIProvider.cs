using Microsoft.Extensions.Options;
using OpenAI.Chat;
using TAO.AI.Abstractions;
using TAO.AI.Common;
using TAO.AI.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.AI.Providers.OpenAI;

internal sealed class OpenAIProvider : ILLMProvider
{
    private readonly ChatClient _chatClient;
    private readonly OpenAIOptions _options;

    public OpenAIProvider(
        IOptions<OpenAIOptions> options)
    {
        _options = options.Value;

        _chatClient = new ChatClient(
            model: _options.Model,
            apiKey: _options.ApiKey);
    }

    public async Task<Result<LLMResponse>> GenerateAsync(
     LLMRequest request,
     CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var messages = new List<ChatMessage>
    {
        ChatMessage.CreateUserMessage(request.Prompt)
    };

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = _options.MaxOutputTokens,

            // We always expect JSON from TAO AI workflows.
          //  ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        try
        {
            var completion = await _chatClient.CompleteChatAsync(
                messages,
                options,
                cancellationToken);

            var content = string.Concat(
                completion.Value.Content.Select(x => x.Text));


            if (string.IsNullOrWhiteSpace(content))
            {
                return Result<LLMResponse>.Failure(
                    AiErrors.InvalidProviderResponse);
            }

            var llmResponse = new LLMResponse
            {
                Content = content,
                ProviderName = "OpenAI",
                ModelName = _options.Model
            };

            return Result<LLMResponse>.Success(llmResponse);
        }
        catch (HttpRequestException)
        {
            return Result<LLMResponse>.Failure(
                AiErrors.ProviderUnavailable);
        }
        catch (TaskCanceledException)
        {
            return Result<LLMResponse>.Failure(
                AiErrors.ProviderTimeout);
        }
        catch (Exception)
        {
            return Result<LLMResponse>.Failure(
                AiErrors.ProviderRequestFailed);
        }
    }
}