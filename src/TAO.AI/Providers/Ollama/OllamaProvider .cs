using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TAO.AI.Abstractions;
using TAO.AI.Common;
using TAO.AI.Contracts;
using TAO.SharedKernel.Results;
using TAO.AI.Providers.Ollama.Contracts;

namespace TAO.AI.Providers.Ollama;

internal sealed class OllamaProvider : ILLMProvider
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaProvider(
        HttpClient httpClient,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<Result<LLMResponse>> GenerateAsync(
    LLMRequest request,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var ollamaRequest = new OllamaGenerateRequest
        {
            Model = _options.Model,
            Prompt = request.Prompt,
            Stream = false,
            Format= "json"
        };

        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(
                "/api/generate",
                ollamaRequest,
                cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return Result<LLMResponse>.Failure(
                    AiErrors.ProviderRequestFailed);
            }

            var ollamaResponse =
                await httpResponse.Content.ReadFromJsonAsync<OllamaGenerateResponse>(
                    cancellationToken: cancellationToken);

            if (ollamaResponse is null)
            {
                return Result<LLMResponse>.Failure(
                    AiErrors.InvalidProviderResponse);
            }

            var llmResponse = new LLMResponse
            {
                Content = ollamaResponse.Response,
                ProviderName = "Ollama",
                ModelName = ollamaResponse.Model
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
    }
}