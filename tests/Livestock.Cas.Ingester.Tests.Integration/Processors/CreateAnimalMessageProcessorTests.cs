using Amazon.SimpleNotificationService;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Ingester.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Cas.Ingester.Tests.Integration.Processors;

public class CreateAnimalMessageProcessorTests(AppTestFixture appTestFixture) : IClassFixture<AppTestFixture>
{
    private readonly AppTestFixture _appTestFixture = appTestFixture;
    private TestObserver<CreateAnimalMessage>? _observer;

    public const string TopicIdentifier = "mav-dev-animal-events";

    [Fact]
    public async Task GivenCreateAnimalMessagePublishedToTopic_WhenReceivedOnTheQueue_ShouldComplete()
    {
        // Arrange
        var cph = Guid.NewGuid();
        var species = Guid.NewGuid();

        var createAnimalMessage = GetCreateAnimalMessage(cph.ToString(), species.ToString());
        
        // Act
        await ExecuteTest(createAnimalMessage);

        // Assert
        var (_, Payload) = await _observer!.MessageHandled;
        Assert.Equal(cph.ToString(), Payload.Cph);
        Assert.Equal(species.ToString(), Payload.Species);
    }

    private async Task ExecuteTest(CreateAnimalMessage message)
    {
        using var cts = new CancellationTokenSource();

        using var scope = _appTestFixture.AppWebApplicationFactory.Server.Services.CreateAsyncScope();
        var amazonSimpleNotificationService = scope.ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();
        _observer = scope.ServiceProvider.GetRequiredService<TestObserver<CreateAnimalMessage>>();

        var allTopics = await amazonSimpleNotificationService.ListTopicsAsync();
        var topic = allTopics.Topics.FirstOrDefault(x => x.TopicArn.EndsWith(TopicIdentifier, StringComparison.InvariantCultureIgnoreCase));

        if (topic != null)
        {
            var publishRequest = SNSMessageUtility.CreateMessage(topic.TopicArn, message);
            await amazonSimpleNotificationService.PublishAsync(publishRequest, cts.Token);
        }

        await _observer.MessageHandled;
    }

    private static CreateAnimalMessage GetCreateAnimalMessage(string cph, string species) => new()
    {
        Cph = cph,
        Species = species
    };
}
