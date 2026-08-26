using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using System.Data.Common;
using TAO.AI.Abstractions;
using TAO.Infrastructure;
using TAO.IntegrationTests.Common.Fakes;

namespace TAO.IntegrationTests.Common;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private string? _connectionString;
    private Respawner? _respawner;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
       "Testing:IgnorePendingModelChanges",
       "true");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ILLMProvider>();
            services.AddSingleton<ILLMProvider, FakeLLMProvider>();
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TaoDbContext>();

        await dbContext.Database.MigrateAsync();

        _connectionString = dbContext.Database.GetConnectionString();

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore =
                    [
                        "__EFMigrationsHistory"
                    ]
            });
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner is null || _connectionString is null)
        {
            throw new InvalidOperationException(
                "Respawner has not been initialized.");
        }

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        await _respawner.ResetAsync(connection);
    }

    public new async Task DisposeAsync()
    {
        if (_connectionString is not null)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.DisposeAsync();
        }
    }
}