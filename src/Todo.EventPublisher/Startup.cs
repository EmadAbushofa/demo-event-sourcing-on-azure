using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Serilog;
using Todo.EventPublisher.Services;

[assembly: FunctionsStartup(typeof(Todo.EventPublisher.Startup))]
namespace Todo.EventPublisher
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSerilog();
            builder.Services.AddServiceBusPublisher();
        }
    }
}
