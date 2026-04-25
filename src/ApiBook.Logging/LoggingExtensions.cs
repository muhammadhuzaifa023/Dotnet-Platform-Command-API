using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ApiBook.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddStructuredLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/apibook-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
        builder.Services.AddSingleton(Log.Logger);

        return builder;
    }

    public static WebApplication UseStructuredRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}
