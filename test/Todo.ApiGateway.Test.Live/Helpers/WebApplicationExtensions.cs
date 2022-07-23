using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System.Net.Http.Headers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Helpers
{
    public static class WebApplicationExtensions
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

        public static HttpClient CreateClientWithUser(this WebApplicationFactory<Program> factory, string user)
        {
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestUser", user);
            return client;
        }

        public static GrpcChannel CreateGrpcChannel(this WebApplicationFactory<Program> factory, string user)
        {
            var client = factory.CreateDefaultClient();
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("Authorization", $"TestUser {user}");
                return Task.CompletedTask;
            });
            return GrpcChannel.ForAddress(
                "https://localhost",
                new GrpcChannelOptions
                {
                    HttpClient = client,
                    Credentials = ChannelCredentials.Create(ChannelCredentials.SecureSsl, credentials)
                });
        }
    }
}