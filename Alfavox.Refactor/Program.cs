using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Serilog.Formatting.Json;

namespace Alfavox.Refactor;

class Program
{
    static async Task Main(string[] args)
    {
        string projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
        string jsonPath = Path.Combine(projectPath, "appsettings.json");
        string logPath = Path.Combine(projectPath, "logs", "app_log.txt");
        string txtPath = Path.Combine(projectPath, "txt", "LukeSkywalkerData.txt");

        // Wczytanie konfiguracji z appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(jsonPath, optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(new JsonFormatter(), logPath)
            .CreateLogger();

        Log.Information("Start aplikacji");

        // Pobranie ustawień API z konfiguracji
        var apiBaseUrl = configuration["ApiSettings:SwapiBaseUrl"];
        if (string.IsNullOrEmpty(apiBaseUrl))
        {
            Log.Fatal("Brak konfiguracji adresu API");
            return;
        }

        var serviceProvider = new ServiceCollection()
             .AddHttpClient()
             .AddSingleton<IHttpClientService, HttpClientService>()
             .AddSingleton<ISwapiService>(provider => 
                new SwapiService(provider.GetRequiredService<IHttpClientService>(), apiBaseUrl))
             .BuildServiceProvider();

        try
        {
            var swapiService = serviceProvider.GetRequiredService<ISwapiService>();
            Log.Information("Rozpoczęto pobieranie danych");

            var data = await swapiService.GetLukeSkywalkerDataAsync();
            Log.Information("Pobrano dane: {@Data}", data);

            await WriteDataToFileAsync(txtPath, data);
            Log.Information("Dane zapisane do pliku {FileName}", Path.GetFileName(txtPath));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Wystąpił błąd podczas pobierania danych.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static async Task WriteDataToFileAsync(string path, LukeSkywalkerData data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        using (var writer = new StreamWriter(path))
        {
            await writer.WriteAsync(JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
