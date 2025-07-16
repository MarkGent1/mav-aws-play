using Amazon.SQS;
using Livestock.Cas.Infrastructure.Messaging;
using Livestock.Cas.Infrastructure.Security.Setup;

namespace Livestock.Cas.Ingester.Setup;

public static class ServiceCollectionExtensions
{
    public static void ConfigureEventListener(this IServiceCollection services)
    {
        // Load certificates into Trust Store - Note must happen before Mongo and Http client connections.
        services.AddCertificates();

        services.AddQueueListenerAsHostedService();
    }

    private static void AddQueueListenerAsHostedService(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonSQS>();

        services.AddHostedService<QueueListener>()
            .AddSingleton<QueueListener>();
    }
}
