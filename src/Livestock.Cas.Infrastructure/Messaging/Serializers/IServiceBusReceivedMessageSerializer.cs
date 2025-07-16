using Amazon.SQS.Model;

namespace Livestock.Cas.Infrastructure.Messaging.Serializers;

public interface IServiceBusReceivedMessageSerializer<out T>
{
    T? Deserialize(Message message);
}
