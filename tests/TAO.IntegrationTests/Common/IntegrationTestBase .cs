using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace TAO.IntegrationTests.Common;

public abstract class IntegrationTestBase
    : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    protected IntegrationTestBase(
        CustomWebApplicationFactory factory)
    {
        Factory = factory;

        Client = factory.CreateClient();

        _scope = factory.Services.CreateScope();

        Services = _scope.ServiceProvider;
    }

    protected HttpClient Client { get; }

    protected IServiceProvider Services { get; }

    protected CustomWebApplicationFactory Factory { get; }

    public void Dispose()
    {
        _scope.Dispose();
    }
}