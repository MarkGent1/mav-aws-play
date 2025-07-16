using Amazon.SimpleNotificationService.Model;

namespace Livestock.Cas.Infrastructure.Messaging;

public interface IMessageFactory
{
    PublishRequest CreateMessage<TBody>(
        string topicArn,
        TBody body,
        string? subject = null,
        string? messageGroupId = null,
        Guid? messageDeduplicationId = null,
        Dictionary<string, string>? additionalUserProperties = null);
}
