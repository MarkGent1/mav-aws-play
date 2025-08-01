using Livestock.Cas.Infrastructure.Security.Setup;
using Livestock.Cas.Infrastructure.Telemetry.Setup;
using Livestock.Cas.Ingester.Setup;
using Serilog;
using System.Diagnostics.CodeAnalysis;

var app = CreateWebApplication(args);
await app.RunAsync();
return;

[ExcludeFromCodeCoverage]
static WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
    ConfigureBuilder(builder);

    var app = builder.Build();
    return SetupApplication(app);
}

[ExcludeFromCodeCoverage]
static void ConfigureBuilder(WebApplicationBuilder builder)
{
    builder.Configuration.AddEnvironmentVariables();

    // Load certificates into Trust Store - Note must happen before Mongo and Http client connections.
    builder.Services.AddCertificates();

    // Configure logging to use the CDP Platform standards.
    builder.Host.UseSerilog(SerilogLoggingExtensions.AddLogging);

    builder.Services.AddHealthChecks();
    builder.Services.ConfigureServiceBus(builder.Configuration);
}

[ExcludeFromCodeCoverage]
static WebApplication SetupApplication(WebApplication app)
{
    app.MapHealthChecks("/health");
    app.MapGet("/", () => "Alive!");

    return app;
}

public partial class Program { }
