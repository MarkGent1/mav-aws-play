using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Ingester.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Cas.Ingester.Tests.Integration.Processors;

public class CreateAnimalMessageProcessorTests(AppTestFixture appTestFixture) : IClassFixture<AppTestFixture>
{
    private readonly AppTestFixture _appTestFixture = appTestFixture;
    private TestObserver<CreateAnimalMessage>? _observer;

    private const string TopicArn = "arn:aws:sns:eu-north-1:000000000000:mav-dev-animal-events";

    [Fact]
    public async Task GivenCreateAnimalMessagePublishedToTopic_WhenReceivedOnTheQueue_ShouldComplete()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var cph = Guid.NewGuid();
        var species = Guid.NewGuid();

        var createAnimalMessage = GetCreateAnimalMessage(cph.ToString(), species.ToString());
        var messageToPublish = SNSMessageUtility.CreateMessage(TopicArn, messageId, createAnimalMessage);

        // Act
        await ExecuteTest(messageToPublish);

        // Assert
        var (MessageId, Payload) = await _observer!.MessageHandled;
        Assert.Equal(messageId.ToString(), MessageId);
        Assert.Equal(cph.ToString(), Payload.Cph);
        Assert.Equal(species.ToString(), Payload.Species);
    }

    private async Task ExecuteTest(PublishRequest originalMessage)
    {
        using var cts = new CancellationTokenSource();

        using var scope = _appTestFixture.AppWebApplicationFactory.Server.Services.CreateAsyncScope();
        var amazonSimpleNotificationService = scope.ServiceProvider.GetRequiredService<IAmazonSimpleNotificationService>();
        _observer = scope.ServiceProvider.GetRequiredService<TestObserver<CreateAnimalMessage>>();

        await amazonSimpleNotificationService.PublishAsync(originalMessage, cts.Token);

        await _observer.MessageHandled;
    }

    private static CreateAnimalMessage GetCreateAnimalMessage(string cph, string species) => new()
    {
        Cph = cph,
        Species = species
    };
}
