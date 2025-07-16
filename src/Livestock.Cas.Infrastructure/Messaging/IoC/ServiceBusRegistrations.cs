using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Cas.Infrastructure.Messaging;

public static class ServiceBusRegistrations
{
    public static IServiceCollection AddServiceBusSenderDependencies(this IServiceCollection services)
    {
        return services
            .AddAWSService<IAmazonSimpleNotificationService>()
            .AddTransient<IMessageFactory, MessageFactory>();
    }

    public static IServiceCollection AddServiceBusReceiverDependencies(this IServiceCollection services)
    {
        return services
            .AddAWSService<IAmazonSQS>();
    }
}
