namespace Livestock.Cas.Ingester.Tests.Component;

public class AppTestFixture : IAsyncDisposable
{
    public readonly HttpClient HttpClient;
    public readonly AppWebApplicationFactory AppWebApplicationFactory;

    public AppTestFixture()
    {
        AppWebApplicationFactory = new AppWebApplicationFactory();
        HttpClient = AppWebApplicationFactory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        if (AppWebApplicationFactory is not null)
        {
            try
            {
                await AppWebApplicationFactory.DisposeAsync();
            }
            catch (TaskCanceledException)
            {
                // Swallow cancellation caused by background pollers during shutdown
            }
        }

        HttpClient?.Dispose();
    }
}
