using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;

namespace Alfavox.Refactor;

// Startup.cs
public static class Startup
{
    public static IServiceProvider ConfigureServices()
    {
        string projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
        string jsonPath = Path.Combine(projectPath, "appsettings.json");
        string logPath = Path.Combine(projectPath, "logs", "app_log.txt");

        // Load configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(jsonPath, optional: false, reloadOnChange: true)
            .Build();

        // Configure logging
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.File(new JsonFormatter(), logPath)
            .CreateLogger();

        // Set up dependency injection
        var serviceCollection = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<AppService>()
            .AddHttpClient()
            .AddSingleton<IHttpClientService, HttpClientService>()
            .AddSingleton<ISwapiService>(provider =>
                new SwapiService(provider.GetRequiredService<IHttpClientService>(),
                                 configuration["ApiSettings:SwapiBaseUrl"] ?? throw new InvalidOperationException("API base URL not configured")));

        return serviceCollection.BuildServiceProvider();
    }
}
