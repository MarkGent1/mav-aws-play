using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Messaging.Serializers;
using System.Text.Json;

namespace Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1.Serializers;

public class CreateAnimalMessageSerializer : IServiceBusReceivedMessageSerializer<CreateAnimalMessage>
{
    public CreateAnimalMessage? Deserialize(Message message)
    {
        var messageBody = JsonSerializer.Deserialize<CreateAnimalMessage>(message.Body, CreateAnimalMessageSerializerContext.Default.CreateAnimalMessage);
        return messageBody;
    }
}
