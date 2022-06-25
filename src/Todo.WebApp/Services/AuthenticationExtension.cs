namespace Todo.WebApp.Services
{
    public static class AuthenticationExtension
    {
        public static void AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
        {
            var accessTokenScope = configuration.GetSection("AzureAd")["AccessTokenScope"];

            services.AddMsalAuthentication(options =>
            {
                configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add(accessTokenScope);
            });
        }
    }
}
