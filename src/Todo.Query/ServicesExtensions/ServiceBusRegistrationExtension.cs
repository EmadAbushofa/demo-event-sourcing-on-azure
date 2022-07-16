using Azure.Messaging.ServiceBus;
using Todo.Query.Abstractions;
using Todo.Query.Infrastructure.ServiceBus;

namespace Todo.Query.ServicesExtensions
{
    public static class ServiceBusRegistrationExtension
    {
        public static void AddServiceBus(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var connectionString = provider.GetRequiredService<IConfiguration>()
                    .GetConnectionString("ServiceBus");

                return new ServiceBusClient(connectionString);
            });

            services.AddSingleton<IMessagePublisher, ServiceBusPublisher>();
        }
    }
}
