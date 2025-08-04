using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Messaging.Serializers;
using System.Text.Json;

namespace Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1.Serializers;

public class UpdateAnimalMessageSerializer : IServiceBusReceivedMessageSerializer<UpdateAnimalMessage>
{
    public UpdateAnimalMessage? Deserialize(Message message)
    {
        var envelope = JsonSerializer.Deserialize<SnsEnvelope>(message.Body);
        var messageBody = JsonSerializer.Deserialize(envelope!.Message, UpdateAnimalMessageSerializerContext.Default.UpdateAnimalMessage);
        return messageBody;
    }
}
