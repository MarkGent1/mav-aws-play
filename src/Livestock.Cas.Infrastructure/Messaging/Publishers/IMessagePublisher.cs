namespace Livestock.Cas.Infrastructure.Messaging.Publishers;

public interface IMessagePublisher<in T>
{
    string TopicIdentifier { get; }

    Task PublishAsync(T? message, CancellationToken cancellationToken = default);
}
