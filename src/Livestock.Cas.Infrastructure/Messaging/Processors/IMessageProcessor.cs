namespace Livestock.Cas.Infrastructure.Messaging.Processors;

public interface IMessageProcessor<in T>
{
    string QueueName { get; }

    Task ProcessMessageAsync(T? message, CancellationToken cancellationToken = default);
}
