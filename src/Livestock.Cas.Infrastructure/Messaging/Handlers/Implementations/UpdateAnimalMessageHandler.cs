using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Messaging.Serializers;

namespace Livestock.Cas.Infrastructure.Messaging.Handlers.Implementations;

public class UpdateAnimalMessageHandler(IServiceBusReceivedMessageSerializer<UpdateAnimalMessage> serializer)
    : IMessageHandler<UpdateAnimalMessage>
{
    private readonly IServiceBusReceivedMessageSerializer<UpdateAnimalMessage> _serializer = serializer;

    public async Task<MessageType> Handle(Message message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var messagePayload = _serializer.Deserialize(message);

        // Do something with the messagePayload

        return await Task.FromResult(messagePayload!);
    }
}
