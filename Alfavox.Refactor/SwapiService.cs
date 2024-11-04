using Newtonsoft.Json;

namespace Alfavox.Refactor;

public class SwapiService : ISwapiService
{
    private readonly IHttpClientService _httpClientService;

    public SwapiService(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task<LukeSkywalkerData> GetLukeSkywalkerDataAsync()
    {
        string response = await _httpClientService.GetStringAsync("https://swapi.py4e.com/api/people/1/");
        var lukeData = JsonConvert.DeserializeObject<LukeSkywalkerData>(response);

        lukeData.FilmNames = await GetNamesFromUrlsAsync(lukeData.Films);
        lukeData.VehicleNames = await GetNamesFromUrlsAsync(lukeData.Vehicles);
        lukeData.StarshipNames = await GetNamesFromUrlsAsync(lukeData.Starships);

        return lukeData;
    }

    private async Task<List<string>> GetNamesFromUrlsAsync(List<string> urls)
    {
        var names = new List<string>();

        foreach (var url in urls)
        {
            string response = await _httpClientService.GetStringAsync(url);
            var name = JsonConvert.DeserializeObject<dynamic>(response).title ?? 
                JsonConvert.DeserializeObject<dynamic>(response).name;
            names.Add(name.ToString());
        }

        return names;
    }
}
