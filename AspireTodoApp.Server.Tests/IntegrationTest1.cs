using Microsoft.Extensions.Logging;

namespace AspireTodoApp.Server.Tests;

public class IntegrationTest1 : IAsyncLifetime
{
    
    
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
    private DistributedApplication _app = null!;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireTodoApp_AppHost>();

        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Warning);
            logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
        });

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await appHost.BuildAsync().WaitAsync(DefaultTimeout);
        await _app.StartAsync().WaitAsync(DefaultTimeout);
    }

    public async Task DisposeAsync() => await _app.DisposeAsync();

    [Fact]
    public async Task ServerHealthCheckReturnsOk()
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);

        await _app.ResourceNotifications
            .WaitForResourceHealthyAsync("server", cts.Token)
            .WaitAsync(DefaultTimeout);

        using var client = _app.CreateHttpClient("server");
        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task WeatherForecastReturnsOk()
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);

        await _app.ResourceNotifications
            .WaitForResourceHealthyAsync("server", cts.Token)
            .WaitAsync(DefaultTimeout);

        using var client = _app.CreateHttpClient("server");
        using var response = await client.GetAsync("/api/weatherforecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
