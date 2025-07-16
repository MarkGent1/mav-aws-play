using Amazon.SimpleNotificationService;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;

namespace Livestock.Cas.Infrastructure.Messaging.Publishers.Implementations;

public class CreateAnimalMessagePublisher(IAmazonSimpleNotificationService amazonSimpleNotificationService,
    IMessageFactory messageFactory) : IMessagePublisher<CreateAnimalMessage>
{
    private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService = amazonSimpleNotificationService;
    private readonly IMessageFactory _messageFactory = messageFactory;

    public string TopicArn => "arn:aws:sns:eu-north-1:558379060554:mav-dev-animal-events.fifo";

    public async Task PublishAsync(CreateAnimalMessage? message, CancellationToken cancellationToken = default)
    {
        if (message == null) return;

        try
        {
            var publishRequest = _messageFactory.CreateMessage(TopicArn, message);
            await _amazonSimpleNotificationService.PublishAsync(publishRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new PublishFailedException($"Failed to publish event on {TopicArn}.", false, ex);
        }
    }
}
