using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Todo.WebApp.Services
{
    public static class ServerApiClientRegistrationExtension
    {
        public static void AddServerApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["ServerUrl"];

            services.AddHttpClient("ServerApi", client =>
                client.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
                .ConfigureHandler(
                    authorizedUrls: new[] { baseUrl }));

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("ServerApi"));
        }
    }
}
