namespace Alfavox.Refactor;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> GetStringAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
