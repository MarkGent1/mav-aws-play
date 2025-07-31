using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Cas.Infrastructure.Messaging;

public static class ServiceBusRegistrations
{
    public static IServiceCollection AddServiceBusSenderDependencies(this IServiceCollection services)
    {
        services.AddTransient<IMessageFactory, MessageFactory>();

        // services.AddAWSService<IAmazonSimpleNotificationService>();

        services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
        {
            var config = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = "http://localstack:4566",
                UseHttp = true,
                AuthenticationRegion = "eu-north-1"
            };

            return new AmazonSimpleNotificationServiceClient(config);
        });

        return services;
    }

    public static IServiceCollection AddServiceBusReceiverDependencies(this IServiceCollection services)
    {
        // services.AddAWSService<IAmazonSQS>();

        services.AddSingleton<IAmazonSQS>(sp =>
        {
            var config = new AmazonSQSConfig
            {
                ServiceURL = "http://localstack:4566",
                UseHttp = true,
                AuthenticationRegion = "eu-north-1"
            };

            var credentials = new BasicAWSCredentials("test", "test");
            return new AmazonSQSClient(credentials, config);
        });

        return services;
    }
}
