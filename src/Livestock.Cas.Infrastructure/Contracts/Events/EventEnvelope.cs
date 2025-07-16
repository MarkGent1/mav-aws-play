namespace Livestock.Cas.Infrastructure.Contracts.Events;

public class EventEnvelope<T>
{
    public string EventType { get; set; } = typeof(T).Name;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;
    public T? Payload { get; set; }
}
