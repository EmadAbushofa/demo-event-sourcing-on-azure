using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Todo.WebApp.TodoProto.Channel;

namespace Todo.WebApp.Services
{
    public static class GrpcWebExtension
    {
        public static void AddGrpcWebClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(provider =>
            {
                var accessTokenProvider = provider.GetRequiredService<IAccessTokenProvider>();

                var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
                {
                    try
                    {
                        var result = await accessTokenProvider.RequestAccessToken();

                        if (result.TryGetToken(out var accessToken))
                        {
                            metadata.Add("Authorization", $"Bearer {accessToken.Value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                var baseUri = configuration["ServerUrl"];

                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
                {
                    HttpClient = httpClient,
                    Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
                });
                return new TasksChannel.TasksChannelClient(channel);
            });
        }
    }
}
