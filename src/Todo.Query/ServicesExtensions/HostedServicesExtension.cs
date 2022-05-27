using Todo.Query.Infrastructure.Data;

namespace Todo.Query.ServicesExtensions
{
    public static class HostedServicesExtension
    {
        public static void AddHostedServices(this IServiceCollection services)
        {
            RunDatabase(services);
        }

        private static void RunDatabase(IServiceCollection services)
            => services.AddHostedService<DatabaseRunner>();
    }
}
