using Todo.Query.Infrastructure.Data;
using Todo.Query.Infrastructure.ServiceBus;

namespace Todo.Query.ServicesExtensions
{
    public static class HostedServicesExtension
    {
        public static void AddHostedServices(this IServiceCollection services)
        {
            RunDatabase(services);
            ListenToEvents(services);
        }

        private static void RunDatabase(IServiceCollection services)
            => services.AddHostedService<DatabaseRunner>();

        private static void ListenToEvents(IServiceCollection services)
            => services.AddHostedService<TodoListener>();
    }
}
