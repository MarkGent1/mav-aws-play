using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Messaging.Listeners;
using Livestock.Cas.Ingester.Tests.Component.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Livestock.Cas.Ingester.Tests.Component.Handlers;

public class UpdateAnimalMessageHandlerTests(AppTestFixture appTestFixture) : IClassFixture<AppTestFixture>
{
    private readonly AppTestFixture _appTestFixture = appTestFixture;
    private TestObserver<MessageType>? _observer;

    [Fact]
    public async Task GivenUpdateAnimalMessage_ShouldComplete()
    {
        // Arrange
        var messageId = Guid.NewGuid().ToString();
        var cph = Guid.NewGuid();
        var species = Guid.NewGuid();

        var updateAnimalMessage = GetUpdateAnimalMessage(cph.ToString(), species.ToString());
        var messageArgs = GetMessageArgs(messageId, updateAnimalMessage);
        var receiveMessageResponseArgs = GetReceiveMessageResponseArgs(messageArgs);

        // Act
        await ExecuteTest(receiveMessageResponseArgs);

        // Assert
        SQSMessageUtility.VerifyMessageWasCompleted(_appTestFixture.AppWebApplicationFactory.SQSClientMock);

        var (MessageId, Payload) = await _observer!.MessageHandled;
        var payloadAsType = Payload as UpdateAnimalMessage;
        Assert.Equal(messageId, MessageId);
        Assert.Equal(cph.ToString(), payloadAsType!.Cph);
        Assert.Equal(species.ToString(), payloadAsType!.Species);
    }

    private async Task ExecuteTest(ReceiveMessageResponse receiveMessageResponseArgs)
    {
        using var cts = new CancellationTokenSource();

        _appTestFixture.AppWebApplicationFactory.SQSClientMock
            .SetupSequence(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(receiveMessageResponseArgs)
            .ReturnsAsync(new ReceiveMessageResponse { HttpStatusCode = System.Net.HttpStatusCode.OK, Messages = [] });

        using var scope = _appTestFixture.AppWebApplicationFactory.Server.Services.CreateAsyncScope();
        var queuePollerMultiType = scope.ServiceProvider.GetRequiredService<IQueuePollerMultiType>();

        _observer = scope.ServiceProvider.GetRequiredService<TestObserver<MessageType>>();

        await queuePollerMultiType.StartAsync(cts.Token);

        await _observer.MessageHandled;
    }

    private static UpdateAnimalMessage GetUpdateAnimalMessage(string cph, string species) => new()
    {
        Cph = cph,
        Species = species
    };

    private static ReceiveMessageResponse GetReceiveMessageResponseArgs(Message message)
    {
        var receiveMessageResponse = SQSMessageUtility.CreateReceiveMessageResponse(message);
        return receiveMessageResponse;
    }

    private static Message GetMessageArgs(string messageId, UpdateAnimalMessage updateAnimalMessage)
    {
        var message = SQSMessageUtility.SetupMessageWithOriginSns(messageId, updateAnimalMessage);
        return message;
    }
}
