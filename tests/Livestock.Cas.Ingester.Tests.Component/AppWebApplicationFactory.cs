using Amazon.SQS;
using Amazon.SQS.Model;
using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Messaging.Observers;
using Livestock.Cas.Ingester.Tests.Component.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Livestock.Cas.Ingester.Tests.Component;

public class AppWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IAmazonSQS> SQSClientMock = new() { DefaultValue = DefaultValue.Mock };

    private IHost? host;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveService<IAmazonSQS>(services);
            RemoveService<IHealthCheckPublisher>(services);
            
            ConfigureServiceBus(services);
        });

        host = base.CreateHost(builder);

        return host;
    }

    private void ConfigureServiceBus(IServiceCollection services)
    {
        SQSClientMock
            .Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueueUrlResponse { QueueUrl = Guid.NewGuid().ToString() });

        SQSClientMock
            .Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse { HttpStatusCode = System.Net.HttpStatusCode.OK, Messages = new List<Message>() });

        SQSClientMock
            .Setup(x => x.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteMessageResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });

        services.AddSingleton(SQSClientMock.Object);

        services.AddSingleton<TestObserver<CreateAnimalMessage>>();
        services.AddScoped<IQueuePollerObserver<CreateAnimalMessage>>(sp => sp.GetRequiredService<TestObserver<CreateAnimalMessage>>());
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
