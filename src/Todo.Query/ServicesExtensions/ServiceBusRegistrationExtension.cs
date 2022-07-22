using Azure.Messaging.ServiceBus;

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
        }
    }
}
