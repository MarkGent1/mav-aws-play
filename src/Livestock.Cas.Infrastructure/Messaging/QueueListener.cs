using Amazon.SQS;
using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Exceptions;
using Livestock.Cas.Infrastructure.Messaging.Processors;
using Livestock.Cas.Infrastructure.Messaging.Serializers;
using Microsoft.Extensions.Hosting;

namespace Livestock.Cas.Infrastructure.Messaging;

public class QueueListener<T>(IAmazonSQS amazonSQS,
    IMessageProcessor<T> messageProcessor,
    IServiceBusReceivedMessageSerializer<T> serializer) : IHostedService
{
    private readonly IMessageProcessor<T> _messageProcessor = messageProcessor;
    private readonly IAmazonSQS _amazonSQS = amazonSQS;
    private readonly IServiceBusReceivedMessageSerializer<T> _serializer = serializer;

    private Task? _pollingTask;
    private CancellationTokenSource? _cts;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _pollingTask = Task.Run(() => PollMessagesAsync(_cts.Token), cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();

        if (_pollingTask != null)
        {
            await _pollingTask;
        }
    }

    private async Task PollMessagesAsync(CancellationToken cancellationToken)
    {
        var queueUrl = await _amazonSQS.GetQueueUrlAsync(_messageProcessor.QueueName, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl.QueueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20,
                    MessageAttributeNames = ["All"]
                };

                var response = await _amazonSQS.ReceiveMessageAsync(request, cancellationToken);

                foreach (var message in response.Messages)
                {
                    await HandleMessageAsync(message, queueUrl.QueueUrl, cancellationToken);
                }
            }
            catch (Exception)
            {
                // "Polling error: {message}"

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private async Task HandleMessageAsync(Message message, string queueUrl, CancellationToken cancellationToken)
    {
        try
        {
            var messagePayload = _serializer.Deserialize(message);

            await _messageProcessor.ProcessMessageAsync(messagePayload, cancellationToken);

            await _amazonSQS.DeleteMessageAsync(queueUrl, message.ReceiptHandle, cancellationToken);
        }
        catch (RetryableException)
        {
            // "RetryableException in queue: {queue}, messageId: {messageId}" / "SQS doesn't support abandon so let visibility timeout expire".
        }
        catch (NonRetryableException)
        {
            // "NonRetryableException in queue: {queue}, messageId: {messageId}" / "Move to a DLQ by configuration".
        }
        catch (Exception)
        {
            // "Unhandled Exception in queue: {queue}, messageId: {messageId}".
        }
    }
}
