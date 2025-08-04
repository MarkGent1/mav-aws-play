using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Messaging.Ioc;
using Livestock.Cas.Infrastructure.Messaging.Publishers;
using Livestock.Cas.Infrastructure.Messaging.Publishers.Implementations;
using Livestock.Cas.Infrastructure.Persistence.Setup;
using Livestock.Cas.Infrastructure.Security.Setup;

namespace Livestock.Cas.Api.Setup;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Load certificates into Trust Store - Note must happen before Mongo and Http client connections.
        services.AddCertificates();

        services.AddMongoDb();

        services.AddDefaultAWSOptions(configuration.GetAWSOptions());

        services.AddServiceBusSenderDependencies(configuration);

        services.AddServiceBusMessagePublishers();
    }

    public static IServiceCollection AddServiceBusMessagePublishers(this IServiceCollection services)
    {
        services.AddSingleton<IMessagePublisher<CreateAnimalMessage>, CreateAnimalMessagePublisher>();

        return services;
    }
}
