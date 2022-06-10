using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.EventPublisher.Services
{
    public static class ServiceBusRegisterExtension
    {
        public static void AddServiceBusPublisher(this IServiceCollection services)
        {
            services.AddSingleton<ServiceBusPublisherOptions>();

            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<ServiceBusPublisherOptions>();

                return new ServiceBusClient(options.ConnectionString);
            });

            services.AddSingleton<ServiceBusPublisher>();
        }
    }
}
