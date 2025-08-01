using Amazon.SimpleNotificationService.Model;
using Livestock.Cas.Infrastructure;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1.Serializers;
using System.Text.Json;

namespace Livestock.Cas.Ingester.Tests.Integration.Helpers;

public class SNSMessageUtility
{
    internal static PublishRequest CreateMessage<TBody>(
        string topicArn,
        TBody body)
    {
        var messageType = typeof(TBody).Name;

        return GenerateMessage(
            topicArn,
            SerializeToJson(body),
            messageType,
            additionalUserProperties: null);
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
