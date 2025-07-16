using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using System.Text.Json;

namespace Livestock.Cas.Infrastructure.Messaging;

public class MessagePublisher
{
    const string TopicArn = "";

    public async Task SendAsync(string name)
    {
        var snsClient = new AmazonSimpleNotificationServiceClient();

        await PublishToTopicAsync(snsClient, TopicArn, name);

        await Task.FromResult(true);
    }

    public static async Task PublishToTopicAsync(
        IAmazonSimpleNotificationService client,
        string topicArn,
        string name)
    {
        var envelope = new MessageEnvelope<CreateAnimalMessage>
        {
            Payload = new CreateAnimalMessage
            {
                Cph = "XX/XXXX/XX",
                Species = name
            }
        };
        var messageJson = JsonSerializer.Serialize(envelope);

        var request = new PublishRequest
        {
            TopicArn = topicArn,
            MessageGroupId = envelope.MessageType,
            Message = messageJson,
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "CorrelationId", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = envelope.CorrelationId
                    }
                }
            },
            MessageDeduplicationId = envelope.CorrelationId
        };

        await client.PublishAsync(request);
    }
}
