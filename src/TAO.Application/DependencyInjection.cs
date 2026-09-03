using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TAO.Application.AssessmentQuestions.Services;
using TAO.Application.AssessmentStrategies.Services;
using TAO.Application.Common.Behaviors;
using TAO.Application.Common.Interfaces;
using TAO.Application.Email;
using TAO.Application.ResumeImports.Services;
using TAO.Application.ResumeScreenings.Services;

namespace TAO.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        services.AddScoped<IResumeImportProcessor, ResumeImportProcessor>();

        services.AddScoped<IResumeParser, ResumeParser>();

        services.AddScoped<IResumeScreeningMarkdownGenerator,
            ResumeScreeningMarkdownGenerator>();

        services.AddScoped<IAssessmentStrategyMarkdownGenerator,
            AssessmentStrategyMarkdownGenerator>();

        services.AddScoped<IAssessmentQuestionGenerationService,
            AssessmentQuestionGenerationService>();

        services.Configure<EmailOptions>(
            configuration.GetSection(EmailOptions.SectionName));

        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}