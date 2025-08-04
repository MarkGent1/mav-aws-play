using Amazon.SQS;
using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure;
using Livestock.Cas.Infrastructure.Contracts.Messages;
using Moq;
using System.Text.Json;

namespace Livestock.Cas.Ingester.Tests.Component.Helpers;

public class SQSMessageUtility
{
    internal static ReceiveMessageResponse CreateReceiveMessageResponse(Message message)
    {
        var receiveMessageResponse = new ReceiveMessageResponse { HttpStatusCode = System.Net.HttpStatusCode.OK, Messages = [message] };
        return receiveMessageResponse;
    }

    internal static ReceiveMessageResponse CreateReceiveMessageResponse(List<Message> messages)
    {
        var receiveMessageResponse = new ReceiveMessageResponse { HttpStatusCode = System.Net.HttpStatusCode.OK, Messages = messages };
        return receiveMessageResponse;
    }

    internal static Message SetupMessageWithOriginSns<TMessage>(string messageId, TMessage message)
    {
        var messageSerialized = JsonSerializer.Serialize(message, JsonDefaults.DefaultOptionsWithStringEnumConversion);
        var snsEnvelope = new SnsEnvelope { Message = messageSerialized, MessageId = messageId };
        var snsEnvelopeSerialized = JsonSerializer.Serialize(snsEnvelope, JsonDefaults.PropertyNamingPolicyAndWriteIndented);
        var serviceBusMessage = new Message { MessageId = messageId, ReceiptHandle = messageId, Body = snsEnvelopeSerialized, MessageAttributes = [] };
        serviceBusMessage.MessageAttributes.TryAdd("Subject", new MessageAttributeValue
        {
            DataType = "String",
            StringValue = typeof(TMessage).Name.Replace("Message", string.Empty)
        });
        return serviceBusMessage;
    }

    internal static void VerifyMessageWasProcessed(Mock<IAmazonSQS>? sqsClientMock)
    {
        sqsClientMock?.Verify(x => x.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    internal static void VerifyMessageWasCompleted(Mock<IAmazonSQS>? sqsClientMock)
    {
        sqsClientMock?.Verify(x => x.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
