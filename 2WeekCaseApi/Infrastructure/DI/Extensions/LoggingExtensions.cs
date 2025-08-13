using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

// using Microsoft.Extensions.Hosting;

namespace Infrastructure.DI.Extensions;

public static class LoggingExtensions
{
    public static void AddSerilogLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        hostBuilder.UseSerilog();
    }
}