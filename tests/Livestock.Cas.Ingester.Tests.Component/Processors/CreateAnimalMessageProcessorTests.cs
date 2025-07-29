using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Messaging;
using Livestock.Cas.Ingester.Tests.Component.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Livestock.Cas.Ingester.Tests.Component.Processors;

public class CreateAnimalMessageProcessorTests(AppTestFixture appTestFixture) : IClassFixture<AppTestFixture>
{
    private readonly AppTestFixture _appTestFixture = appTestFixture;
    private TestObserver<CreateAnimalMessage>? _observer;

    [Fact]
    public async Task GivenCreateAnimalMessage_ShouldComplete()
    {
        // Arrange
        var messageId = Guid.NewGuid().ToString();
        var cph = Guid.NewGuid();
        var species = Guid.NewGuid();

        var createAnimalMessage = GetCreateAnimalMessage(cph.ToString(), species.ToString());
        var messageArgs = GetMessageArgs(messageId, createAnimalMessage);
        var receiveMessageResponseArgs = GetReceiveMessageResponseArgs(messageArgs);

        // Act
        await ExecuteTest(receiveMessageResponseArgs);

        // Assert
        SQSMessageUtility.VerifyMessageWasCompleted(_appTestFixture.AppWebApplicationFactory.SQSClientMock);

        var handledMessage = await _observer!.MessageHandled;
        Assert.Equal(messageId, handledMessage.MessageId);
        Assert.Equal(cph.ToString(), handledMessage.Payload.Cph);
        Assert.Equal(species.ToString(), handledMessage.Payload.Species);
    }

    private async Task ExecuteTest(ReceiveMessageResponse receiveMessageResponseArgs)
    {
        using var cts = new CancellationTokenSource();
        
        _appTestFixture.AppWebApplicationFactory.SQSClientMock
            .SetupSequence(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(receiveMessageResponseArgs)
            .ReturnsAsync(new ReceiveMessageResponse { HttpStatusCode = System.Net.HttpStatusCode.OK, Messages = new List<Message>() });

        using var scope = _appTestFixture.AppWebApplicationFactory.Server.Services.CreateAsyncScope();
        var queuePoller = scope.ServiceProvider.GetRequiredService<IQueuePoller<CreateAnimalMessage>>();
        _observer = scope.ServiceProvider.GetRequiredService<TestObserver<CreateAnimalMessage>>();

        await queuePoller.StartAsync(cts.Token);

        await _observer.MessageHandled;
    }

    private static CreateAnimalMessage GetCreateAnimalMessage(string cph, string species) => new()
    {
        Cph = cph,
        Species = species
    };

    private static ReceiveMessageResponse GetReceiveMessageResponseArgs(Message message)
    {
        var receiveMessageResponse = SQSMessageUtility.CreateReceiveMessageResponse(message);
        return receiveMessageResponse;
    }

    private static Message GetMessageArgs(string messageId, CreateAnimalMessage createAnimalMessage)
    {
        var message = SQSMessageUtility.SetupMessageWithOriginSns(messageId, createAnimalMessage);
        return message;
    }
}
