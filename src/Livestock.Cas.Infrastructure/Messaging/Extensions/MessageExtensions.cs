using Amazon.SQS.Model;

namespace Livestock.Cas.Infrastructure.Messaging.Extensions;

public static class MessageExtensions
{
    public static string? GetMessageSubjectFromAttributes(this Message message)
    {
        string? subject = null;

        if (message.MessageAttributes.TryGetValue("Subject", out var subjectAttr))
        {
            subject = subjectAttr.StringValue;
        }

        return subject;
    }
}
