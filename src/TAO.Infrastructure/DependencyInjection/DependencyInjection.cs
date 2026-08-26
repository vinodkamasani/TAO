using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TAO.Application.Common.Interfaces;
using TAO.Infrastructure.Persistence;

namespace TAO.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextPool<TaoDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));

            if (configuration.GetValue<bool>(
                "Testing:IgnorePendingModelChanges"))
            {
                options.ConfigureWarnings(
                    warnings =>
                        warnings.Ignore(
                            RelationalEventId.PendingModelChangesWarning));
            }
        });


        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<TaoDbContext>());

        return services;
    }
}