using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace Todo.Command.Test
{
    public static class TestHelper
    {
        public static WebApplicationFactory<Program> WithDefaultConfigurations(this WebApplicationFactory<Program> factory, ITestOutputHelper helper) => factory
               .WithWebHostBuilder(builder =>
               {
                   builder.ConfigureLogging(loggingBuilder =>
                   {
                       Log.Logger = new LoggerConfiguration()
                           .MinimumLevel.Information()
                           .WriteTo.TestOutput(helper, LogEventLevel.Information)
                           .CreateLogger();
                   });

                   builder.ConfigureTestServices(services =>
                   {

                   });
               });

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
    }
}