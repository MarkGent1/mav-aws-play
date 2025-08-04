using Amazon.SQS.Model;
namespace Livestock.Cas.Infrastructure.Messaging.Observers;

public interface IQueuePollerObserver<T>
{
    void OnMessageHandled(string messageId, DateTime handledAt, T payload, Message rawMessage);
    void OnMessageFailed(string messageId, DateTime failedAt, Exception exception, Message rawMessage);
}
