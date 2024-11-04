using Newtonsoft.Json;
using Serilog;

namespace Alfavox.Zadanie;

class Program
{
    static async Task Main(string[] args)
    {
        string projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
        string logPath = Path.Combine(projectPath, "logs", "app_log.txt");
        string txtPath = Path.Combine(projectPath, "txt", "LukeSkywalkerData.txt");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logPath)
            .CreateLogger();

        Log.Information("Start aplikacji");

        try
        {
            await FetchAndSaveLukeSkywalkerDataAsync(txtPath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Wystąpił błąd podczas wykonywania aplikacji");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static async Task FetchAndSaveLukeSkywalkerDataAsync(string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        using var httpClient = new HttpClient();
        var service = new SwapiService(httpClient);

        Log.Information("Rozpoczęto pobieranie danych o Luke'u Skywalkerze");
        var data = await service.GetLukeSkywalkerDataAsync();
        Log.Information("Pobrano dane");

        // Pobierz szczegóły filmów, pojazdów i statków
        data.Films = await GetDetailsAsync(service, data.Films, "filmy");
        data.Vehicles = await GetDetailsAsync(service, data.Vehicles, "pojazdy");
        data.Starships = await GetDetailsAsync(service, data.Starships, "statki");

        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        await File.WriteAllTextAsync(filePath, json);
        Log.Information("Dane zapisane do pliku {FileName}", Path.GetFileName(filePath));
    }

    private static async Task<List<string>> GetDetailsAsync(SwapiService service, List<string> urls, string type)
    {
        Log.Information("Pobieranie szczegółów {Type}", type);
        return await service.GetNamesFromUrlsAsync(urls);
    }
}
