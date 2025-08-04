namespace Livestock.Cas.Infrastructure.Messaging.Listeners;

public interface IQueuePoller<T>
{
    Task StartAsync(CancellationToken token);
    Task StopAsync(CancellationToken token);
}