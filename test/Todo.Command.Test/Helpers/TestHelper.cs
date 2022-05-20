using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Todo.Command.Abstraction;
using Todo.Command.Test.FakeServices;
using Xunit.Abstractions;

namespace Todo.Command.Test.Helpers
{
    public static class TestHelper
    {
        public static WebApplicationFactory<Program> WithDefaultConfigurations(this WebApplicationFactory<Program> factory, ITestOutputHelper helper, Action<IServiceCollection>? servicesConfiguration = null) => factory
               .WithWebHostBuilder(builder =>
               {
                   builder.ConfigureLogging(loggingBuilder =>
                   {
                       Log.Logger = new LoggerConfiguration()
                           .MinimumLevel.Information()
                           .WriteTo.TestOutput(helper, LogEventLevel.Information)
                           .CreateLogger();
                   });

                   if (servicesConfiguration != null)
                       builder.ConfigureTestServices(servicesConfiguration);
               });

        public static void ReplaceWithInMemoryEventStore(this IServiceCollection services)
        {
            var eventStore = services.Single(s => s.ServiceType == typeof(IEventStore));
            services.Remove(eventStore);

            services.AddSingleton<IEventStore, InMemoryEventStore>();
        }

        public static GrpcChannel CreateGrpcChannel(this WebApplicationFactory<Program> factory)
        {
            var client = factory.CreateDefaultClient();
            return GrpcChannel.ForAddress(
                client.BaseAddress ?? throw new InvalidOperationException("BaseAddress is null"),
                new GrpcChannelOptions
                {
                    HttpClient = client
                });
        }

        public static Timestamp ToUtcTimestamp(string dateTime)
            => DateTime.Parse(dateTime).ToUniversalTime().ToTimestamp();
    }
}