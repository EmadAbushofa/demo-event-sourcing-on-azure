using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Todo.ApiGateway.Test.Live.FakeServices;

namespace Todo.ApiGateway.Test.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void UseTestAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", options => { });

            services.AddScoped<IAuthorizationHandler, TestAllowAnonymous>();
        }
    }
}