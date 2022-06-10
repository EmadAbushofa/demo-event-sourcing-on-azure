using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Todo.EventPublisher.Services
{
    public static class LoggerServiceBuilder
    {
        public static void AddSerilog(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                var appName = configuration["SerilogAppName"];

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Worker", LogEventLevel.Warning)
                    .MinimumLevel.Override("Host", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Function", LogEventLevel.Warning)
                    .MinimumLevel.Override("Azure.Storage.Blobs", LogEventLevel.Warning)
                    .MinimumLevel.Override("Azure.Core", LogEventLevel.Warning)
                    .Enrich.WithProperty("Name", appName)
                    .WriteTo.Console()
                    .CreateLogger();

                return new SerilogLoggerProvider(Log.Logger, true);
            });
        }
    }
}
