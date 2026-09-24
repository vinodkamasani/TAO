using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TAO.Application.Common.Interfaces;
using TAO.Infrastructure.Identity;
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

        services.AddDataProtection();
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
     {
         options.User.RequireUniqueEmail = true;

         options.Password.RequireDigit = true;
         options.Password.RequireLowercase = true;
         options.Password.RequireUppercase = true;
         options.Password.RequireNonAlphanumeric = false;
         options.Password.RequiredLength = 8;

         options.Lockout.MaxFailedAccessAttempts = 5;
         options.Lockout.DefaultLockoutTimeSpan =
             TimeSpan.FromMinutes(15);
     })
     .AddEntityFrameworkStores<TaoDbContext>()
     .AddDefaultTokenProviders();


        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "TAO.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // later change it to CookieSecurePolicy.Always in production
            options.Cookie.SameSite = SameSiteMode.Lax;

            options.LoginPath = "/api/auth/login";
            options.AccessDeniedPath = "/api/auth/access-denied";

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<TaoDbContext>());
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITransactionManager, EfTransactionManager>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<
    IUserClaimsPrincipalFactory<ApplicationUser>,
    TaoUserClaimsPrincipalFactory>();

        return services;
    }
}