using System.Text.Json.Serialization;

namespace Livestock.Cas.Infrastructure.Contracts.Messages;

public class MessageType
{
    public MessageType()
    {
        Id = Guid.NewGuid();
    }

    [JsonConstructor]
    public MessageType(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private set; }
}
