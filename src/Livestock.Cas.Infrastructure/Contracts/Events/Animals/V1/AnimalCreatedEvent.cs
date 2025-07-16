namespace Livestock.Cas.Infrastructure.Contracts.Events.Animals.V1;

public record AnimalCreatedEvent
{
    public Guid EventId { get; init; }
    public string Species { get; init; } = "";
    public string Cph { get; init; } = "";
    public DateTime Timestamp { get; init; }
}
