namespace Livestock.Cas.Infrastructure.Contracts.Messages;

public class SnsEnvelope
{
    public string Type { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string TopicArn { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
