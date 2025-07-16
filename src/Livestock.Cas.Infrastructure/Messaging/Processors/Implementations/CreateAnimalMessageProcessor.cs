using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;

namespace Livestock.Cas.Infrastructure.Messaging.Processors.Implementations;

public class CreateAnimalMessageProcessor : IMessageProcessor<CreateAnimalMessage>
{
    public string QueueName => "mav-dev-animals";

    public async Task ProcessMessageAsync(CreateAnimalMessage? message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        // Do something with the message

        await Task.CompletedTask;
    }
}
