using Amazon.SimpleNotificationService.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1.Serializers;
using System.Text;
using System.Text.Json;

namespace Livestock.Cas.Infrastructure.Messaging.Implementations;

//public class MessageFactory : IMessageFactory
//{
//    private const string EventId = "EventId";
//    private const string EventTimeUtc = "EventTimeUtc";

//    /*
//     PublishRequest CreateMessage<TBody>(
//        TBody body,
//        string topicArn,
//        string correlationId,
//        Dictionary<string, MessageAttributeValue>? additionalMessageAttributes = null,
//        string? subject = null)
//     */

//    private static PublishRequest CreateMessage<TBody>(
//        TBody body,
//        Dictionary<string, string>? additionalUserProperties = null,
//        string? subject = null,
//        string? messageGroupId = null,
//        Guid? messageDeduplicationId = null)
//    {
//        var messageEnvelope = new MessageEnvelope<TBody>
//        {
//            Payload = body
//        };

//        /*
//        var request = new PublishRequest
//        {
//            TopicArn = topicArn,
//            MessageGroupId = envelope.MessageType,
//            MessageDeduplicationId = envelope.CorrelationId
//        };
//         */

//        return GenerateMessage(
//            Encoding.UTF8.GetBytes(SerializeToJson(messageEnvelope)),
//            additionalUserProperties,
//            subject);
//    }

//    private static PublishRequest GenerateMessage(
//        byte[] body,
//        Dictionary<string, string>? additionalUserProperties,
//        string? subject = null)
//    {
//        var dateTime = DateTime.UtcNow;

//        var message = new PublishRequest
//        {
//            Subject = subject,
//            MessageAttributes = []
//        };

//        message.MessageAttributes.Add(EventTimeUtc, new MessageAttributeValue
//        {
//            DataType = "String",
//            StringValue = dateTime.ToString()
//        });
//        message.MessageAttributes.Add(EventId, new MessageAttributeValue
//        {
//            DataType = "String",
//            StringValue = Guid.NewGuid().ToString("N")
//        });

//        if (additionalUserProperties == null || !additionalUserProperties.Any())
//        {
//            return message;
//        }

//        foreach (var (key, value) in additionalUserProperties)
//        {
//            message.MessageAttributes.Add(key, new MessageAttributeValue
//            {
//                DataType = "String",
//                StringValue = value
//            });
//        }

//        return message;
//    }

//    private static string SerializeToJson<TBody>(TBody value)
//    {
//        return typeof(TBody) switch
//        {
//            Type t when t == typeof(CreateAnimalMessage) => JsonSerializer.Serialize(value, CreateAnimalMessageSerializerContext.Default.CreateAnimalMessage),
//            _ => JsonSerializer.Serialize(value, JsonDefaults.DefaultOptionsWithStringEnumConversion)
//        };
//    }
//}
