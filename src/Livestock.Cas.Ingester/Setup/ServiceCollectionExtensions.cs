using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1.Serializers;
using Livestock.Cas.Infrastructure.Messaging.Handlers;
using Livestock.Cas.Infrastructure.Messaging.Handlers.Implementations;
using Livestock.Cas.Infrastructure.Messaging.Ioc;
using Livestock.Cas.Infrastructure.Messaging.Listeners;
using Livestock.Cas.Infrastructure.Messaging.Listeners.Implementations;
using Livestock.Cas.Infrastructure.Messaging.Processors;
using Livestock.Cas.Infrastructure.Messaging.Processors.Implementations;
using Livestock.Cas.Infrastructure.Messaging.Serializers;
using Livestock.Cas.Infrastructure.Security.Setup;

namespace Livestock.Cas.Ingester.Setup;

public static class ServiceCollectionExtensions
{
    public static void ConfigureServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        // Load certificates into Trust Store - Note must happen before Mongo and Http client connections.
        services.AddCertificates();

        services.AddServiceBusReceiverDependencies(configuration);

        services.AddQueueListenerAsHostedService<CreateAnimalMessage>();

        services.AddQueueListenerMultiTypeAsHostedService();

        services.AddServiceBusReceivedMessageSerializers();

        services.AddServiceBusMessageProcessors();

        services.AddServiceBusMessageHandlers();
    }

    private static void AddQueueListenerAsHostedService<T>(this IServiceCollection services)
    {
        services.AddHostedService<QueueListener<T>>()
            .AddSingleton<IQueuePoller<T>, QueuePoller<T>>();
    }

    private static void AddQueueListenerMultiTypeAsHostedService(this IServiceCollection services)
    {
        services.AddHostedService<QueueListenerMultiType>()
            .AddSingleton<IQueuePollerMultiType, QueuePollerMultiType>();
    }

    private static IServiceCollection AddServiceBusReceivedMessageSerializers(this IServiceCollection services)
    {
        services.AddSingleton<IServiceBusReceivedMessageSerializer<CreateAnimalMessage>, CreateAnimalMessageSerializer>();
        services.AddSingleton<IServiceBusReceivedMessageSerializer<UpdateAnimalMessage>, UpdateAnimalMessageSerializer>();

        return services;
    }

    private static IServiceCollection AddServiceBusMessageProcessors(this IServiceCollection services)
    {
        services.AddTransient<IMessageProcessor<CreateAnimalMessage>, CreateAnimalMessageProcessor>();

        return services;
    }

    private static IServiceCollection AddServiceBusMessageHandlers(this IServiceCollection services)
    {
        services.AddTransient<IMessageHandler<UpdateAnimalMessage>, UpdateAnimalMessageHandler>();

        var messageHandlerManager = new InMemoryMessageHandlerManager();
        messageHandlerManager.AddReceiver<UpdateAnimalMessage, IMessageHandler<UpdateAnimalMessage>>();

        services.AddSingleton<IMessageHandlerManager>(messageHandlerManager);

        return services;
    }
}
