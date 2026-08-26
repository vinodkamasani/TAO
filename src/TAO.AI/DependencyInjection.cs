using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TAO.AI.Abstractions;
using TAO.AI.AssessmentEvaluations;
using TAO.AI.AssessmentEvaluations.Parsers;
using TAO.AI.AssessmentEvaluations.PromptTemplates;
using TAO.AI.AssessmentEvaluations.Validators;
using TAO.AI.AssessmentFollowUps;
using TAO.AI.AssessmentFollowUps.Parsers;
using TAO.AI.AssessmentFollowUps.PromptTemplates;
using TAO.AI.AssessmentFollowUps.Validators;
using TAO.AI.AssessmentQuestionEvaluations;
using TAO.AI.AssessmentQuestionEvaluations.Parsers;
using TAO.AI.AssessmentQuestionEvaluations.PromptTemplates;
using TAO.AI.AssessmentQuestionEvaluations.Validators;
using TAO.AI.AssessmentQuestions;
using TAO.AI.AssessmentQuestions.Parsers;
using TAO.AI.AssessmentQuestions.PromptTemplates;
using TAO.AI.AssessmentQuestions.Validators;
using TAO.AI.AssessmentRoundEvaluations;
using TAO.AI.AssessmentRoundEvaluations.Parsers;
using TAO.AI.AssessmentRoundEvaluations.PromptTemplates;
using TAO.AI.AssessmentRoundEvaluations.Validators;
using TAO.AI.AssessmentStrategies;
using TAO.AI.AssessmentStrategies.Parsers;
using TAO.AI.AssessmentStrategies.PromptTemplates;
using TAO.AI.AssessmentStrategies.Validators;
using TAO.AI.Common;
using TAO.AI.HiringStrategies;
using TAO.AI.HiringStrategies.Parsers;
using TAO.AI.HiringStrategies.PromptTemplates;
using TAO.AI.HiringStrategies.Validators;
using TAO.AI.JobProfiles;
using TAO.AI.JobProfiles.Parsers;
using TAO.AI.JobProfiles.PromptTemplates;
using TAO.AI.JobProfiles.Validators;
using TAO.AI.Providers.Ollama;
using TAO.AI.Providers.OpenAI;
using TAO.AI.ResumeParsing;
using TAO.AI.ResumeParsing.DocumentExtraction;
using TAO.AI.ResumeParsing.Parsers;
using TAO.AI.ResumeParsing.PromptTemplates;
using TAO.AI.ResumeParsing.Validators;
using TAO.AI.ResumeScreening;
using TAO.AI.ResumeScreening.Parsers;
using TAO.AI.ResumeScreening.PromptTemplates;
using TAO.AI.ResumeScreening.Validators;

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


        // Assesment Strategy
        services.AddScoped<IAssessmentStrategyGenerator, AssessmentStrategyGenerator>();

        services.AddScoped<AssessmentStrategyPromptFactory>();
        services.AddScoped<AssessmentStrategyResponseParser>();
        services.AddScoped<AssessmentStrategyResponseValidator>();


        //Assesment Session

        services.AddScoped<IAssessmentQuestionGenerator,AssessmentQuestionGenerator>();

        services.AddScoped<AssessmentQuestionPromptFactory>();

        services.AddScoped<AssessmentQuestionResponseParser>();

        services.AddScoped<AssessmentQuestionResponseValidator>();

        // Assessment Follow up
        services.AddScoped<IAssessmentFollowUpGenerator, AssessmentFollowUpGenerator>();

        services.AddScoped<AssessmentFollowUpPromptFactory>();

        services.AddScoped<AssessmentFollowUpResponseParser>();

        services.AddScoped<AssessmentFollowUpResponseValidator>();


        // Assessment Question Evaluation

        services.AddScoped<
            IAssessmentQuestionEvaluationGenerator,
            AssessmentQuestionEvaluationGenerator>();

        services.AddScoped<
            AssessmentQuestionEvaluationPromptFactory>();

        services.AddScoped<
            AssessmentQuestionEvaluationResponseParser>();

        services.AddScoped<
            AssessmentQuestionEvaluationResponseValidator>();


        // Assessmen round evaluation
        services.AddScoped<IAssessmentRoundEvaluationGenerator,
    AssessmentRoundEvaluationGenerator>();

        services.AddScoped<
            AssessmentRoundEvaluationPromptFactory>();

        services.AddScoped<
            AssessmentRoundEvaluationResponseParser>();

        services.AddScoped<
            AssessmentRoundEvaluationResponseValidator>();

        //Final evaluation
        services.AddScoped<IAssessmentEvaluationGenerator,
    AssessmentEvaluationGenerator>();

        services.AddScoped<AssessmentEvaluationPromptFactory>();
        services.AddScoped<AssessmentEvaluationResponseParser>();
        services.AddScoped<AssessmentEvaluationResponseValidator>();



        return services;
    }
}