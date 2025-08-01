using Amazon.SimpleNotificationService.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1.Serializers;
using System.Text.Json;

namespace Livestock.Cas.Infrastructure.Messaging;

public class MessageFactory : IMessageFactory
{
    private const string EventId = "EventId";
    private const string EventTimeUtc = "EventTimeUtc";

    public PublishRequest CreateMessage<TBody>(
        string topicArn,
        TBody body,
        string? subject = null,
        string? messageGroupId = null,
        Guid? messageDeduplicationId = null,
        Dictionary<string, string>? additionalUserProperties = null)
    {
        var messageType = typeof(TBody).Name;

        return GenerateMessage(
            topicArn,
            SerializeToJson(body),
            subject ?? messageType,
            additionalUserProperties);
    }

    private static PublishRequest GenerateMessage(
        string topicArn,
        string body,
        string subject,
        Dictionary<string, string>? additionalUserProperties)
    {
        var dateTime = DateTime.UtcNow;

        var message = new PublishRequest(topicArn, body, subject)
        {
            MessageAttributes = []
        };

        message.MessageAttributes.Add(EventTimeUtc, new MessageAttributeValue
        {
            DataType = "String",
            StringValue = dateTime.ToString()
        });

        if (additionalUserProperties == null || additionalUserProperties.Count == 0)
        {
            return message;
        }

        foreach (var (key, value) in additionalUserProperties)
        {
            message.MessageAttributes.Add(key, new MessageAttributeValue
            {
                DataType = "String",
                StringValue = value
            });
        }

        return message;
    }

    private static string SerializeToJson<TBody>(TBody value)
    {
        return typeof(TBody) switch
        {
            Type t when t == typeof(CreateAnimalMessage) => JsonSerializer.Serialize(value, CreateAnimalMessageSerializerContext.Default.CreateAnimalMessage),
            _ => JsonSerializer.Serialize(value, JsonDefaults.DefaultOptionsWithStringEnumConversion)
        };
    }
}
