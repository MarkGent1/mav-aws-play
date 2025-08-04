namespace Livestock.Cas.Infrastructure.Messaging.Listeners;

public interface IQueuePollerMultiType
{
    Task StartAsync(CancellationToken token);
    Task StopAsync(CancellationToken token);
}
