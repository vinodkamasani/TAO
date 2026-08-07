using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TAO.Application.Common.Behaviors;
using TAO.Application.ResumeImports.Services;
using TAO.Application.ResumeScreenings.Services;

namespace TAO.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>),typeof(LoggingBehavior<,>));
        services.AddScoped<IResumeImportProcessor, ResumeImportProcessor>();

        services.AddScoped<IResumeParser, ResumeParser>();
        services.AddScoped<IResumeImportProcessor, ResumeImportProcessor>();
        services.AddScoped<IResumeScreeningMarkdownGenerator, ResumeScreeningMarkdownGenerator>();


        return services;
    }
}