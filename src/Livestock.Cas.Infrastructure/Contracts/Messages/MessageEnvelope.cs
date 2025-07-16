namespace Livestock.Cas.Infrastructure.Contracts.Messages;

public class MessageEnvelope<T>
{
    public string MessageType { get; set; } = typeof(T).Name;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;
    public T? Payload { get; set; }
}
