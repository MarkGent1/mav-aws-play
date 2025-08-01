using Amazon.SimpleNotificationService;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;

namespace Livestock.Cas.Infrastructure.Messaging.Publishers.Implementations;

public class CreateAnimalMessagePublisher(IAmazonSimpleNotificationService amazonSimpleNotificationService,
    IMessageFactory messageFactory) : IMessagePublisher<CreateAnimalMessage>
{
    private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService = amazonSimpleNotificationService;
    private readonly IMessageFactory _messageFactory = messageFactory;

    public string TopicIdentifier => "mav-dev-animal-events";

    public async Task PublishAsync(CreateAnimalMessage? message, CancellationToken cancellationToken = default)
    {
        if (message == null) return;

        try
        {
            var allTopics = await _amazonSimpleNotificationService.ListTopicsAsync(cancellationToken);
            var topic = allTopics.Topics.FirstOrDefault(x => x.TopicArn.EndsWith(TopicIdentifier, StringComparison.InvariantCultureIgnoreCase));

            if (topic != null)
            {
                var publishRequest = _messageFactory.CreateMessage(topic.TopicArn, message);
                await _amazonSimpleNotificationService.PublishAsync(publishRequest, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw new PublishFailedException($"Failed to publish event on {TopicIdentifier}.", false, ex);
        }
    }
}
