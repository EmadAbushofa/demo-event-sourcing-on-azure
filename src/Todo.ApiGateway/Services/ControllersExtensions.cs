using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;
using Todo.ApiGateway.ActionFilters;

namespace Todo.ApiGateway.Services
{
    public static class ControllersExtensions
    {
        public static void AddControllersWithConfigurations(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions
                    .Add(new SlugifyControllerNamesConvention());

                options.Filters.Add<HttpResponseExceptionFilter>();
            });
        }

        private class SlugifyControllerNamesConvention : RouteTokenTransformerConvention
        {
            public SlugifyControllerNamesConvention() : base(new SlugifyParameterTransformer())
            {

            }

            private class SlugifyParameterTransformer : IOutboundParameterTransformer
            {
                public string? TransformOutbound(object? value)
                {
                    var stringValue = value?.ToString();

                    // Slugify value
                    return stringValue == null
                        ? null
                        : Regex.Replace(stringValue, "([a-z])([A-Z])", "$1-$2").ToLower();
                }
            }
        }
    }
}
