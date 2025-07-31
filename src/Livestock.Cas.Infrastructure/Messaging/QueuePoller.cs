using Amazon.SQS;
using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Exceptions;
using Livestock.Cas.Infrastructure.Messaging.Observers;
using Livestock.Cas.Infrastructure.Messaging.Processors;
using Livestock.Cas.Infrastructure.Messaging.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Cas.Infrastructure.Messaging;

public class QueuePoller<T>(IServiceScopeFactory scopeFactory,
    IAmazonSQS amazonSQS,
    IMessageProcessor<T> messageProcessor,
    IServiceBusReceivedMessageSerializer<T> serializer) : IQueuePoller<T>, IAsyncDisposable
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IAmazonSQS _amazonSQS = amazonSQS;
    private readonly IMessageProcessor<T> _messageProcessor = messageProcessor;
    private readonly IServiceBusReceivedMessageSerializer<T> _serializer = serializer;

    private IQueuePollerObserver<T>? _observer;

    private Task? _pollingTask;
    private CancellationTokenSource? _cts;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        _observer = scope.ServiceProvider.GetService<IQueuePollerObserver<T>>();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        Task.Delay(TimeSpan.FromSeconds(1));

        _pollingTask = Task.Run(() => PollMessagesAsync(_cts.Token), cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();

        if (_pollingTask is { IsCompletedSuccessfully: false })
        {
            await _pollingTask;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_cts is not null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        if (_pollingTask is { IsCompleted: false })
        {
            try
            {
                await _pollingTask;
            }
            catch (TaskCanceledException)
            {
                // Swallow expected task cancellation during disposal
            }
        }
    }

    private async Task PollMessagesAsync(CancellationToken cancellationToken)
    {
        string queueUrl = "";
        
        try
        {
            var queueResponse = await _amazonSQS.GetQueueUrlAsync(_messageProcessor.QueueName, cancellationToken);
            queueUrl = queueResponse.QueueUrl;
        }
        catch (Exception ex)
        {
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var response = await _amazonSQS.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20,
                    MessageAttributeNames = ["All"]
                }, cancellationToken);

                if (response?.Messages?.Any() == true)
                {
                    foreach (var message in response.Messages)
                    {
                        await HandleMessageAsync(message, queueUrl, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
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

            _observer?.OnMessageHandled(message.MessageId, DateTime.UtcNow, messagePayload!, message);
        }
        catch (RetryableException ex)
        {
            // "RetryableException in queue: {queue}, messageId: {messageId}" / "SQS doesn't support abandon so let visibility timeout expire".

            _observer?.OnMessageFailed(message.MessageId, DateTime.UtcNow, ex, message);
        }
        catch (NonRetryableException ex)
        {
            // "NonRetryableException in queue: {queue}, messageId: {messageId}" / "Move to a DLQ by configuration".

            _observer?.OnMessageFailed(message.MessageId, DateTime.UtcNow, ex, message);
        }
        catch (Exception ex)
        {
            // "Unhandled Exception in queue: {queue}, messageId: {messageId}".

            _observer?.OnMessageFailed(message.MessageId, DateTime.UtcNow, ex, message);
        }
    }
}
