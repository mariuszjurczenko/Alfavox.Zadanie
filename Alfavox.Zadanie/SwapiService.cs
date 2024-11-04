using Newtonsoft.Json;

namespace Alfavox.Zadanie;

public class SwapiService
{
    private readonly HttpClient _httpClient;

    public SwapiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LukeSkywalkerData> GetLukeSkywalkerDataAsync()
    {
        var response = await _httpClient.GetStringAsync("https://swapi.py4e.com/api/people/1/");
        return JsonConvert.DeserializeObject<LukeSkywalkerData>(response);
    }

    public async Task<string> GetNameFromUrlAsync(string url)
    {
        var response = await _httpClient.GetStringAsync(url);
        var details = JsonConvert.DeserializeObject<DetailData>(response);
        return details.Name;
    }

    public async Task<List<string>> GetNamesFromUrlsAsync(List<string> urls)
    {
        var names = new List<string>();

        foreach (var url in urls)
        {
            var name = await GetNameFromUrlAsync(url);
            names.Add(name);
        }
        return names;
    }
}

public class DetailData
{
    [JsonProperty("title")]
    public string Name { get; set; } // Używane dla filmów

    [JsonProperty("name")]
    public string AlternateName { set { if (!string.IsNullOrEmpty(value)) Name = value; } } // Używane dla pojazdów/statków
}
