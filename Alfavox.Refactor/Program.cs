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
        string logPath = Path.Combine(projectPath, "logs", "app_log.txt");
        string txtPath = Path.Combine(projectPath, "txt", "LukeSkywalkerData.txt");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(new JsonFormatter(), logPath)
            .CreateLogger();

        Log.Information("Start aplikacji");

        var serviceProvider = new ServiceCollection()
            .AddHttpClient()
            .AddSingleton<IHttpClientService, HttpClientService>()
            .AddSingleton<ISwapiService, SwapiService>()
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
