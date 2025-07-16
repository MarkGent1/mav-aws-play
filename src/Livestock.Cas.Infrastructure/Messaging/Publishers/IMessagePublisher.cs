namespace Livestock.Cas.Infrastructure.Messaging.Publishers;

public interface IMessagePublisher<in T>
{
    string TopicArn { get; }

    Task PublishAsync(T? message, CancellationToken cancellationToken = default);
}
