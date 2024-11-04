namespace Alfavox.Refactor;

public interface IHttpClientService
{
    Task<string> GetStringAsync(string url);
}
