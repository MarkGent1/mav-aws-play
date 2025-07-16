using Amazon.SimpleNotificationService.Model;

namespace Livestock.Cas.Infrastructure.Messaging;

public interface IMessageFactory
{
    PublishRequest CreateMessage<TBody>(
        TBody body,
        string topicArn,
        string correlationId,
        Dictionary<string, MessageAttributeValue>? additionalMessageAttributes = null,
        string? subject = null);
}
