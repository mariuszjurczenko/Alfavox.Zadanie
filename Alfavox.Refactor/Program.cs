using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Alfavox.Refactor;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = Startup.ConfigureServices();
        var appService = serviceProvider.GetRequiredService<AppService>();

        try
        {
            await appService.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error occurred.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
