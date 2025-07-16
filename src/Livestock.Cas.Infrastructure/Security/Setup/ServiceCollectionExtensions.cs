using Livestock.Cas.Infrastructure.Security.Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Cas.Infrastructure.Security.Setup;

public static class ServiceCollectionExtensions
{
    public static void AddCertificates(this IServiceCollection services)
    {
        services.AddCustomTrustStore();
    }
}
