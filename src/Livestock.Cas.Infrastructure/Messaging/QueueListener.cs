using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;

namespace Livestock.Cas.Infrastructure.Messaging;

public class QueueListener : IHostedService
{
    const string QueueNameShort = "mav-dev-animals";

    private readonly IAmazonSQS _sqsClient;

    private Task? _pollingTask;
    private CancellationTokenSource? _cts;

    public QueueListener(IAmazonSQS amazonSQS)
    {
        _sqsClient = amazonSQS;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _pollingTask = Task.Run(() => PollMessagesAsync(_cts.Token));
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
        var queueUrl = await _sqsClient.GetQueueUrlAsync(QueueNameShort, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl.QueueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20,
                    AttributeNames = ["All"],
                    MessageAttributeNames = ["All"]
                };

                var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);

                foreach (var message in response.Messages)
                {
                    await HandleMessageAsync(message, queueUrl.QueueUrl, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // _telemetryClient.TrackException(ex);
                // _logger.LogError(ex, "Polling error: {Message}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private async Task HandleMessageAsync(Message message, string queueUrl, CancellationToken cancellationToken)
    {
        try
        {
            // var deserializedMessage = _serializer.Deserialize(message);
            // await _messageProcessor.ProcessMessageAsync(deserializedMessage, cancellationToken);

            await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, cancellationToken);
        }
        //catch (RetryableException ex)
        //{
        //    _telemetryClient.TrackException(ex);
        //    _logger.LogError(ex, "RetryableException in queue: {Queue}, messageId: {MessageId}", _messageProcessor.QueueName, message.MessageId);
        //    // SQS doesn't support abandon — let visibility timeout expire
        //}
        //catch (NonRetryableException ex)
        //{
        //    _telemetryClient.TrackException(ex);
        //    _logger.LogError(ex, "NonRetryableException in queue: {Queue}, messageId: {MessageId}", _messageProcessor.QueueName, message.MessageId);
        //    // Optionally move to a DLQ by configuration
        //}
        catch (Exception ex)
        {
            // _telemetryClient.TrackException(ex);
            // _logger.LogError(ex, "Unhandled Exception in queue: {Queue}, messageId: {MessageId}", _messageProcessor.QueueName, message.MessageId);
        }
    }
}
