using Amazon.SimpleNotificationService.Model;
using Livestock.Cas.Infrastructure.Messaging;

namespace Livestock.Cas.Ingester.Tests.Integration.Helpers;

public class SNSMessageUtility
{
    internal static PublishRequest CreateMessage<TBody>(
        string topicArn,
        Guid messageId,
        TBody body)
    {
        var messageFactory = new MessageFactory();

        return messageFactory.CreateMessage(topicArn, body, messageDeduplicationId: messageId);
    }
}
