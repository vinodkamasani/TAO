using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TAO.AI.Abstractions;
using TAO.AI.HiringStrategies;
using TAO.AI.HiringStrategies.Parsers;
using TAO.AI.HiringStrategies.PromptTemplates;
using TAO.AI.HiringStrategies.Validators;
using TAO.AI.JobProfiles;
using TAO.AI.JobProfiles.Parsers;
using TAO.AI.JobProfiles.PromptTemplates;
using TAO.AI.JobProfiles.Validators;
using TAO.AI.Providers.Ollama;
using TAO.AI.ResumeParsing;
using TAO.AI.ResumeParsing.DocumentExtraction;
using TAO.AI.ResumeParsing.Parsers;
using TAO.AI.ResumeParsing.PromptTemplates;
using TAO.AI.ResumeParsing.Validators;
using TAO.AI.ResumeScreening;
using TAO.AI.ResumeScreening.Parsers;
using TAO.AI.ResumeScreening.PromptTemplates;
using TAO.AI.ResumeScreening.Validators;
using TAO.AI.Common;
using TAO.AI.Providers.OpenAI;

namespace TAO.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ------------------------------------------------------------------
        // AI Provider Configuration
        // ------------------------------------------------------------------

        services.Configure<AIOptions>(
            configuration.GetSection(AIOptions.SectionName));

        services.Configure<OllamaOptions>(
            configuration.GetSection(OllamaOptions.SectionName));

        services.Configure<OpenAIOptions>(
            configuration.GetSection(OpenAIOptions.SectionName));

        var provider =
            configuration.GetSection(AIOptions.SectionName)
                .Get<AIOptions>()?.Provider
            ?? "OpenAI";

        if (provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<ILLMProvider, OpenAIProvider>();
        }
        else
        {
            services.AddHttpClient<ILLMProvider, OllamaProvider>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<OllamaOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = options.Timeout;
                });
        }

        services.AddTransient<IJobProfileGenerator, JobProfileGenerator>();

        services.AddTransient<JobProfilePromptFactory>();

        services.AddTransient<JobProfileResponseParser>();

        services.AddTransient<JobProfileResponseValidator>();

        // Hiring stratergies servies 
        services.AddScoped<IHiringStrategyGenerator, HiringStrategyGenerator>();

        services.AddScoped<HiringStrategyPromptFactory>();

        services.AddScoped<HiringStrategyResponseParser>();

        services.AddScoped<HiringStrategyResponseValidator>();


        // Resume Parsing
        services.AddScoped<IResumeParserGenerator, ResumeParserGenerator>();
        services.AddScoped<IDocumentTextExtractor, DocumentTextExtractor>();

        services.AddScoped<ResumePromptFactory>();
        services.AddScoped<ResumeResponseParser>();
        services.AddScoped<ResumeResponseValidator>();


        // Resume Screening

        services.AddScoped<IResumeScreeningGenerator, ResumeScreeningGenerator>();

        services.AddSingleton<ResumeScreeningPromptFactory>();

        services.AddSingleton<ResumeScreeningResponseParser>();

        services.AddSingleton<ResumeScreeningResponseValidator>();

        return services;
    }
}