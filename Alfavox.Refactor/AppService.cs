using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Alfavox.Refactor;

public class AppService
{
    private readonly ISwapiService _swapiService;
    private readonly IConfiguration _configuration;

    public AppService(ISwapiService swapiService, IConfiguration configuration)
    {
        _swapiService = swapiService;
        _configuration = configuration;
    }

    public async Task RunAsync()
    {
        Log.Information("Application started.");

        var data = await _swapiService.GetLukeSkywalkerDataAsync();
        Log.Information("Data retrieved: {@Data}", data);

        var outputPath = Path.Combine(GetProjectDirectory(), "txt", "LukeSkywalkerData.txt");
        await WriteDataToFileAsync(outputPath, data);

        Log.Information("Data saved to file: {FileName}", Path.GetFileName(outputPath));
    }

    private string GetProjectDirectory() =>
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

    private async Task WriteDataToFileAsync(string path, LukeSkywalkerData data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var writer = new StreamWriter(path);
        await writer.WriteAsync(JsonConvert.SerializeObject(data, Formatting.Indented));
    }
}
