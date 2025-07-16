using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Livestock.Cas.Infrastructure.Persistence.Setup;

public static class ServiceCollectionExtensions
{
    [ExcludeFromCodeCoverage]
    public static void AddMongoDb(this IServiceCollection services)
    {
        // TODO
    }
}

