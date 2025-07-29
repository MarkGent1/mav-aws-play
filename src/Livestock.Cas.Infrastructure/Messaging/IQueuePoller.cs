namespace Livestock.Cas.Infrastructure.Messaging;

public interface IQueuePoller<T>
{
    Task StartAsync(CancellationToken token);
    Task StopAsync(CancellationToken token);
}