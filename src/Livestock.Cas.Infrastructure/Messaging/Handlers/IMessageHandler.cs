using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages;

namespace Livestock.Cas.Infrastructure.Messaging.Handlers;

public interface IMessageHandler<in TMessage> : IMessageHandler
    where TMessage : MessageType
{
    Task<MessageType> Handle(Message message, CancellationToken cancellationToken = default);
}

public interface IMessageHandler
{
}
