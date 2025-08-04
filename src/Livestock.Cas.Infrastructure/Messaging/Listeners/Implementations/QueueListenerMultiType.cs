using Microsoft.Extensions.Hosting;

namespace Livestock.Cas.Infrastructure.Messaging.Listeners.Implementations;

public class QueueListenerMultiType(IQueuePollerMultiType queuePoller) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => queuePoller.StartAsync(cancellationToken);
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await queuePoller.StopAsync(cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Swallow expected cancellation
        }
    }
}
