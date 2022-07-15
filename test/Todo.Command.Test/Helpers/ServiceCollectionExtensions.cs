using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions.Persistence;
using Todo.Command.Test.FakeServices;

namespace Todo.Command.Test.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void ReplaceWithInMemoryEventStore(this IServiceCollection services)
        {
            var eventStore = services.Single(s => s.ServiceType == typeof(IEventStore));
            services.Remove(eventStore);

            services.AddSingleton<IEventStore, InMemoryEventStore>();
        }
    }
}