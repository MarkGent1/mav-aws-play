namespace Livestock.Cas.Infrastructure.Messaging.Configuration;

public record ServiceBusReceiverConfiguration
{
    public required string QueueName { get; set; }
}
