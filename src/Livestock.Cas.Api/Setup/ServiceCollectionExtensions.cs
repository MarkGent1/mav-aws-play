using Livestock.Cas.Infrastructure.Persistence.Setup;
using Livestock.Cas.Infrastructure.Security.Setup;

namespace Livestock.Cas.Api.Setup;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApi(this IServiceCollection services)
    {
        // Load certificates into Trust Store - Note must happen before Mongo and Http client connections.
        services.AddCertificates();

        services.AddMongoDb();
    }
}
