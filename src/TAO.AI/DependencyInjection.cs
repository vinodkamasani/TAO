using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TAO.AI.Abstractions;
using TAO.AI.JobProfiles;
using TAO.AI.JobProfiles.Parsers;
using TAO.AI.JobProfiles.PromptTemplates;
using TAO.AI.JobProfiles.Validators;
using TAO.AI.Providers.Ollama;

namespace TAO.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OllamaOptions>(
            configuration.GetSection(OllamaOptions.SectionName));

        services.AddHttpClient<ILLMProvider, OllamaProvider>(
      (serviceProvider, client) =>
      {
          var options = serviceProvider
              .GetRequiredService<IOptions<OllamaOptions>>()
              .Value;

          client.BaseAddress = new Uri(options.BaseUrl);
          client.Timeout = options.Timeout;
      });

        services.AddTransient<IJobProfileGenerator, JobProfileGenerator>();

        services.AddTransient<JobProfilePromptFactory>();

        services.AddTransient<JobProfileResponseParser>();

        services.AddTransient<JobProfileResponseValidator>();

        return services;
    }
}