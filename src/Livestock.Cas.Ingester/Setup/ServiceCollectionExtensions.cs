using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1.Serializers;
using Livestock.Cas.Infrastructure.Messaging;
using Livestock.Cas.Infrastructure.Messaging.Processors;
using Livestock.Cas.Infrastructure.Messaging.Processors.Implementations;
using Livestock.Cas.Infrastructure.Messaging.Serializers;
using Livestock.Cas.Infrastructure.Security.Setup;

namespace Livestock.Cas.Ingester.Setup;

public static class ServiceCollectionExtensions
{
    public static void ConfigureServiceBus(this IServiceCollection services)
    {
        // Load certificates into Trust Store - Note must happen before Mongo and Http client connections.
        services.AddCertificates();

        services.AddServiceBusReceiverDependencies();

        services.AddQueueListenerAsHostedService<CreateAnimalMessage>();

        services.AddServiceBusReceivedMessageSerializers();

        services.AddServiceBusMessageProcessors();
    }

    private static void AddQueueListenerAsHostedService<T>(this IServiceCollection services)
    {
        services.AddHostedService<QueueListener<T>>()
            .AddSingleton<QueueListener<T>>();
    }

    private static IServiceCollection AddServiceBusReceivedMessageSerializers(this IServiceCollection services)
    {
        services.AddSingleton<IServiceBusReceivedMessageSerializer<CreateAnimalMessage>, CreateAnimalMessageSerializer>();

        return services;
    }

    private static IServiceCollection AddServiceBusMessageProcessors(this IServiceCollection services)
    {
        services.AddTransient<IMessageProcessor<CreateAnimalMessage>, CreateAnimalMessageProcessor>();

        return services;
    }
}
