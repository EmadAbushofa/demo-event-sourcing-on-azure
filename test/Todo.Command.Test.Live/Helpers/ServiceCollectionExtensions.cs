using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Test.Live.EventBus;

namespace Todo.Command.Test.Live.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServiceBusListener(this IServiceCollection services)
        {
            AddServiceBusClient(services);

            AddListenerAndListenerOptions(services);
        }

        private static void AddServiceBusClient(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                var connectionString = configuration["ServiceBus:ConnectionString"];

                return new ServiceBusClient(connectionString);
            });
        }

        private static void AddListenerAndListenerOptions(IServiceCollection services)
        {
            services.AddSingleton<TodoCommandListener>();

            services.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                // An Azure function will publish the events to a topic with the same name of the container
                var container = configuration["CosmosDb:ContainerId"];

                return new TodoCommandListenerOptions()
                {
                    TopicName = container,
                    SubscriptionName = container,
                };
            });
        }
    }
}