using Amazon.SimpleNotificationService;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Messaging.Observers;
using Livestock.Cas.Infrastructure.Messaging.Publishers;
using Livestock.Cas.Infrastructure.Messaging.Publishers.Implementations;
using Livestock.Cas.Ingester.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Livestock.Cas.Ingester.Tests.Integration;

public class AppWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? host;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveService<IHealthCheckPublisher>(services);

            // services.AddAWSService<IAmazonSimpleNotificationService>();

            services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            {
                var config = new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = "http://localhost:4566",
                    UseHttp = true,
                    AuthenticationRegion = "eu-north-1"
                };

                return new AmazonSimpleNotificationServiceClient(config);
            });

            services.AddSingleton<TestObserver<CreateAnimalMessage>>();
            services.AddScoped<IQueuePollerObserver<CreateAnimalMessage>>(sp => sp.GetRequiredService<TestObserver<CreateAnimalMessage>>());
        });

        host = base.CreateHost(builder);

        return host;
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var service = services.FirstOrDefault(x => x.ServiceType == typeof(T));
        if (service != null)
        {
            services.Remove(service);
        }
    }
}
