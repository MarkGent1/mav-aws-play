using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Cas.Infrastructure.Messaging;

public static class ServiceBusRegistrations
{
    public static IServiceCollection AddServiceBusSenderDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMessageFactory, MessageFactory>();

        if (configuration["AWS:OverrideServiceURL"] != null)
        {
            services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            {
                var config = new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = configuration["AWS:ServiceURL"],
                    AuthenticationRegion = configuration["AWS:Region"],
                    UseHttp = true
                };
                return new AmazonSimpleNotificationServiceClient(config);
            });
        }
        else
        {
            services.AddAWSService<IAmazonSimpleNotificationService>();
        }
        
        return services;
    }

    public static IServiceCollection AddServiceBusReceiverDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration["AWS:OverrideServiceURL"] != null)
        {
            services.AddSingleton<IAmazonSQS>(sp =>
            {
                var config = new AmazonSQSConfig
                {
                    ServiceURL = configuration["AWS:ServiceURL"],
                    AuthenticationRegion = configuration["AWS:Region"],
                    UseHttp = true
                };
                var credentials = new Amazon.Runtime.BasicAWSCredentials("test", "test");
                return new AmazonSQSClient(credentials, config);
            });
        }
        else
        {
            services.AddAWSService<IAmazonSQS>();
        }

        return services;
    }
}
