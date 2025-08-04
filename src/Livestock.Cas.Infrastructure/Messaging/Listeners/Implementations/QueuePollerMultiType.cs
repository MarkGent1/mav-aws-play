using Amazon.SQS;
using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages;
using Livestock.Cas.Infrastructure.Exceptions;
using Livestock.Cas.Infrastructure.Messaging.Configuration;
using Livestock.Cas.Infrastructure.Messaging.Extensions;
using Livestock.Cas.Infrastructure.Messaging.Handlers;
using Livestock.Cas.Infrastructure.Messaging.Observers;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Cas.Infrastructure.Messaging.Listeners.Implementations;

public class QueuePollerMultiType(IServiceScopeFactory scopeFactory,
    IAmazonSQS amazonSQS,
    IMessageHandlerManager messageHandlerManager,
    ServiceBusReceiverConfiguration serviceBusConfiguration) : IQueuePollerMultiType, IAsyncDisposable
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IAmazonSQS _amazonSQS = amazonSQS;
    private readonly IMessageHandlerManager _messageHandlerManager = messageHandlerManager;
    private readonly ServiceBusReceiverConfiguration _serviceBusConfiguration = serviceBusConfiguration;

    private IQueuePollerObserver<MessageType>? _observer;

    private Task? _pollingTask;
    private CancellationTokenSource? _cts;

    private const string MESSAGE_SUFFIX = "Message";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        _observer = scope.ServiceProvider.GetService<IQueuePollerObserver<MessageType>>();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

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

        GC.SuppressFinalize(this);
    }

    private async Task PollMessagesAsync(CancellationToken cancellationToken)
    {
        var queueResponse = await _amazonSQS.GetQueueUrlAsync(_serviceBusConfiguration.QueueName, cancellationToken);
        var queueUrl = queueResponse.QueueUrl;

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

                if (response?.Messages?.Count > 0)
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
            var messageSubject = message.GetMessageSubjectFromAttributes() ?? "Default";

            var handlerTypes = _messageHandlerManager.GetHandlersForMessage(messageSubject);

            foreach (var handlerInfo in handlerTypes)
            {
                var messageType = _messageHandlerManager.GetMessageTypeByName($"{messageSubject}{MESSAGE_SUFFIX}");

                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetService(handlerInfo.HandlerType);
                if (handler == null) continue;

                var concreteType = typeof(IMessageHandler<>).MakeGenericType(messageType);
                var messagePayload = await (Task<MessageType>)concreteType.GetMethod("Handle")!.Invoke(handler, [message, cancellationToken])!;

                await _amazonSQS.DeleteMessageAsync(queueUrl, message.ReceiptHandle, cancellationToken);
                _observer?.OnMessageHandled(message.MessageId, DateTime.UtcNow, messagePayload, message);
            }
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
