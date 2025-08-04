using Livestock.Cas.Infrastructure.Contracts.Messages;

namespace Livestock.Cas.Infrastructure.Messaging.Handlers.Implementations;

public class InMemoryMessageHandlerManager : IMessageHandlerManager
{
    private readonly Dictionary<string, List<MessageHandlerInfo>> handlers;
    private readonly List<Type> messageTypes;

    private const string MESSAGE_SUFFIX = "Message";

    public InMemoryMessageHandlerManager()
    {
        handlers = [];
        messageTypes = [];
    }

    public void AddReceiver<T, TH>()
        where T : MessageType
        where TH : IMessageHandler<T>
    {
        var messageType = GetMessageTypeKey<T>();

        DoAddReceiver(typeof(TH), messageType, isDynamic: false);

        if (!messageTypes.Contains(typeof(T)))
        {
            messageTypes.Add(typeof(T));
        }
    }

    public Type GetMessageTypeByName(string messageType) => messageTypes.SingleOrDefault(t => t.Name == messageType)!;

    public string GetMessageTypeKey<T>()
    {
        var messageName = typeof(T).Name;
        messageName = ReplaceSuffix(messageName, MESSAGE_SUFFIX);
        return messageName;
    }

    public bool HasHandlerForMessage(string messageType) => handlers.ContainsKey(messageType);

    public bool HasHandlerForMessage<T>() where T : MessageType
    {
        var key = GetMessageTypeKey<T>();
        return HasHandlerForMessage(key);
    }

    public IEnumerable<MessageHandlerInfo> GetHandlersForMessage(string messageType) => handlers[messageType];

    public IEnumerable<MessageHandlerInfo> GetHandlersForMessage<T>() where T : MessageType
    {
        var key = GetMessageTypeKey<T>();
        return GetHandlersForMessage(key);
    }

    private void DoAddReceiver(Type handlerType, string messageType, bool isDynamic)
    {
        if (!HasHandlerForMessage(messageType))
        {
            handlers.Add(messageType, []);
        }

        if (handlers[messageType].Any(s => s.HandlerType == handlerType))
        {
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already registered for '{messageType}'", nameof(handlerType));
        }

        if (isDynamic)
        {
            handlers[messageType].Add(MessageHandlerInfo.Dynamic(handlerType));
        }
        else
        {
            handlers[messageType].Add(MessageHandlerInfo.Typed(handlerType));
        }
    }

    private static string ReplaceSuffix(string messageName, string suffix)
    {
        if (messageName.EndsWith(suffix))
        {
            messageName = messageName[..messageName.LastIndexOf(suffix)];
        }
        return messageName;
    }
}
